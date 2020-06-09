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
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class RifleFiredWhileMovingManager : SinglePlayerSituationManager<RifleFiredWhileMoving>
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
        /// Maximum time between the theoretical moment a weapon could have fired the next bullet and the actual moment the bullet was fired to count as a single spray.
        /// 
        /// Reason: Time is inaccurate and players sometimes click very fast instead of spraying, which may still be a misplay.
        /// </summary>
        private const int SINGLE_BURST_TOLERANCE = 200;

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
        public override SkillDomain SkillDomain => SkillDomain.Movement;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.RifleFiredWhileMoving;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<RifleFiredWhileMoving>> TableSelector => context => context.RifleFiredWhileMoving;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<RifleFiredWhileMoving>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var equipmentHelper = _sp.GetRequiredService<IEquipmentHelper>();
                var equipmentDict = equipmentHelper.GetEquipmentDict(data.MatchStats);

                // compute bursts from all weapons that were fired in the match
                var weaponFireds = data.WeaponFiredList
                    // Remove shots of irrelevant weapons
                    .Where(x => AnalyzedWeapons.Contains(x.Weapon));

                var bursts = BurstHelper.DivideIntoBursts(weaponFireds, equipmentDict, SINGLE_BURST_TOLERANCE)
                    .Where(x => x.WeaponFireds.Count >= MIN_SHOTS).ToList();

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

    }
}
