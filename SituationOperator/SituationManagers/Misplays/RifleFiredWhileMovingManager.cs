using EquipmentLib;
using MatchEntities;
using MatchEntities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// Manager for players shooting a burst with a rifle inaccurately because they were moving.
    /// </summary>
    public class RifleFiredWhileMovingManager : SituationManager<RifleFiredWhileMoving>
    {
        /// <summary>
        /// Minimum fraction of inaccurate shots in the burst to count as a misplay.
        /// </summary>
        private const double MIN_INACCURATE_SHOT_FRACTION = 0.7;

        /// <summary>
        /// Maximum time passed without damage being dealt by or to the player after end of burst to count as a misplay.
        /// This setting is there to filter out situations where the player just shoots for fun / at the end of round.
        /// 
        /// Set value to -1 to ignore this condition.
        /// </summary>
        private const int MAX_TIME_BEFORE_FIGHT = 2000;

        /// <summary>
        /// Minimum number of shots fired in quick succession to count as a burst.
        /// </summary>
        private const int MIN_SHOTS = 3;

        /// <summary>
        /// Collection of weapons for which bursts will be analyzed.
        /// </summary>
        private static List<EquipmentElement> AnalyzedWeapons => new List<EquipmentElement> {
                EquipmentElement.Famas,
                EquipmentElement.Gallil,
                EquipmentElement.AK47,
                EquipmentElement.M4A1,
                EquipmentElement.M4A4,
                EquipmentElement.SG556,
                EquipmentElement.AUG,
                EquipmentElement.Scar20,
                EquipmentElement.G3SG1,
                EquipmentElement.Deagle
                //AWP currently dows not count as we track min shots fired for a burst
                //AWP cant fire enough shots to account for a burst
                //EquipmentElement.AWP,
        };

        private readonly IServiceProvider _sp;
        private readonly ILogger<RifleFiredWhileMovingManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RifleFiredWhileMovingManager(IServiceProvider sp, ILogger<RifleFiredWhileMovingManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.RifleFiredWhileMoving;


        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<RifleFiredWhileMoving>> TableSelector => context => context.RifleFiredWhileMoving;

        /// <summary>
        /// Returns all RifleFiredWhileMovings.
        /// </summary>
        /// <param name="data">Data of the match in which to look for situations for all players.</param>
        /// <returns></returns>
        protected override async Task<IEnumerable<RifleFiredWhileMoving>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var equipmentProvider = _sp.GetRequiredService<IEquipmentProvider>();
                var equipmentSet = equipmentProvider.GetEquipmentSet((EquipmentLib.Enums.Source)data.MatchStats.Source, data.MatchStats.MatchDate);

                // compute bursts from all weapons that were fired in the match
                var bursts = new List<Burst>();
                var weaponFiredGroups = data.WeaponFiredList
                    // Group by player and weapon
                    .GroupBy(x => new { x.PlayerId, x.Weapon })
                    // Remove shots of irrelevant weapons
                    .Where(x => AnalyzedWeapons.Contains(x.Key.Weapon));
                foreach (var weaponFiredGroup in weaponFiredGroups)
                {
                    var equipmentInfo = equipmentSet.EquipmentDict[(short)weaponFiredGroup.Key.Weapon];
                    bursts.AddRange(DivideIntoBursts(weaponFiredGroup, MIN_SHOTS, equipmentInfo));
                }

                // create misplays from bursts that fulfill the specified conditions
                var misplays = new List<RifleFiredWhileMoving>();
                foreach (var burst in bursts)
                {
                    if (burst.WeaponFireds.Count < MIN_SHOTS)
                        continue;

                    if ((double) burst.InaccurateBullets / burst.WeaponFireds.Count <= MIN_INACCURATE_SHOT_FRACTION)
                        continue;

                    // ignore bursts where the player took place in no time during or after the burst
                    var fightTimeFrameStart = burst.WeaponFireds.First().Time;
                    var fightTimeFrameEnd = burst.WeaponFireds.Last().Time + MAX_TIME_BEFORE_FIGHT;

                    if(!data.PlayerDealtOrTookDamage(burst.PlayerId, startTime: fightTimeFrameStart, endTime: fightTimeFrameEnd)) 
                    {
                        continue;
                    }

                    var misplay = new RifleFiredWhileMoving(burst.WeaponFireds.First(), burst.WeaponFireds.Count, burst.InaccurateBullets);

                    misplays.Add(misplay);
                }

                return misplays;
            }
        }


        private List<Burst> DivideIntoBursts(IEnumerable<WeaponFired> weaponFireds, int minShots, EquipmentInfo equipmentInfo)
        {
            var bursts = new List<Burst>();
            var weaponFiredsByRound = weaponFireds.GroupBy(x => x.Round);

            foreach (var weaponFiredInRound in weaponFiredsByRound)
            {
                foreach (var wf in weaponFiredInRound.OrderBy(x => x.Time))
                {
                    // If there are no bursts or assigning this wf to the previous burst does not work, start a new burst. 
                    if (bursts.Count() == 0 || !bursts.Last().TryAdd(wf))
                    {
                        bursts.Add(new Burst(wf, equipmentInfo));
                        continue;
                    }
                }
            }

            return bursts.Where(x => x.WeaponFireds.Count >= minShots).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        public class Burst
        {
            /// <summary>
            /// Used to add some extra allowed time between shots to count as burst, to allow for inaccurate data or players tapping 
            /// very very quickly yet not shooting at the exact CycleTime.
            /// </summary>
            private const double EXTEND_CYCLETIME_FACTOR = 1.15;

            public long PlayerId { get; set; }
            public short Round { get; set; }
            public EquipmentElement Weapon { get; set; }
            public List<WeaponFired> WeaponFireds { get; set; } = new List<WeaponFired>();

            /// <summary>
            /// Maximum velocity a player can move at to shoot accurately with this weapon. 
            /// </summary>
            private double MaxAccurateVelocity { get; set; }

            /// <summary>
            /// Maximum time the weapon needs between two shots.
            /// </summary>
            private double MaxAllowedTimeBetweenShots { get; set; }

            public int InaccurateBullets => WeaponFireds.Count(x => x.PlayerVelo.Length() > MaxAccurateVelocity);
            public Burst(WeaponFired weaponFired, EquipmentInfo equipmentInfo)
            {
                PlayerId = weaponFired.PlayerId;
                Round = weaponFired.Round;
                Weapon = weaponFired.Weapon;
                WeaponFireds.Add(weaponFired);

                MaxAccurateVelocity = (double)equipmentInfo.MaxPlayerSpeed / 3;

                MaxAllowedTimeBetweenShots = equipmentInfo.CycleTime * EXTEND_CYCLETIME_FACTOR;
            }

            public bool TryAdd(WeaponFired wf)
            {
                if (wf.Time < WeaponFireds.Last().Time)
                {
                    throw new ArgumentException("WeaponFireds have to be added in chronological order.");
                }

                var belongsToThisBurst =
                    wf.Round == Round
                    && wf.Weapon == Weapon
                    && WeaponFireds.Last().Time + MaxAllowedTimeBetweenShots >= wf.Time;

                if (belongsToThisBurst)
                {
                    WeaponFireds.Add(wf);
                    return true;
                }

                return false;
            }
        }
    }
}
