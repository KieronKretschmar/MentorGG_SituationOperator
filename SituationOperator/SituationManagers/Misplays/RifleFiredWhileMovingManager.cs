using EquipmentLib;
using MatchEntities;
using MatchEntities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Extensions;
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
        /// 
        /// Either this condition or INACCURATE_SHOTS_MIN_FRACTION needs to hold.
        /// </summary>
        private const int INACCURATE_SHOTS_MIN_BULLETS = 4;

        /// <summary>
        /// Minimum fraction of inaccurate shots in the burst to count as a misplay.
        /// 
        /// Either this condition or INACCURATE_SHOTS_MIN_BULLETS needs to hold.
        /// </summary>
        private const double INACCURATE_SHOTS_MIN_FRACTION = 0.45;

        /// <summary>
        /// Minimum fraction of the weapon's possible maximum velocity a player must have moved at to count a bullet as inaccurate.
        /// 
        /// Value is a bit higher than the real value of 0.34 to ignore hard to decide cases that may look like false positives.
        /// </summary>
        private const double MIN_SPEED_FRACTION_TO_COUNT_AS_INACCURATE = 0.40;

        /// <summary>
        /// Maximum time passed without damage being dealt by or to the player after end of burst to count as a misplay.
        /// This setting is there to filter out situations where the player just shoots for fun / at the end of round.
        /// 
        /// Set value to -1 to ignore this condition.
        /// </summary>
        private const int MAX_TIME_BEFORE_FIGHT = 2000;

        /// <summary>
        /// Minimum distance the fighting opponent must have been away from the player to count as a misplay.
        /// </summary>
        private const double MIN_ENEMY_DISTANCE = 4.5;

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
        /// If the player's Line of Sight was blocked by a smoke, this is the minimum length in meters from the smokes center and the player's position
        /// in order to count as a misplay.
        /// 
        /// Set to -1 to ignore this condition.
        /// 
        /// Reason: Spraying into a smoke while changing positions does not count as a misplay, as it may discourage enemies from pushing or land a lucky hit.
        /// </summary>
        private const int MIN_DISTANCE_FROM_SMOKE_CENTER = 7;

        /// <summary>
        /// Whether or not to require the player not being flashed at the start of his burst in order to count as a misplay.
        /// </summary>
        private const bool REQUIRE_NOT_FLASHED = true;


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
                var burstHelper = _sp.GetRequiredService<IBurstHelper>();

                // compute bursts from all weapons that were fired in the match
                var weaponFireds = data.WeaponFiredList
                    // Remove shots of irrelevant weapons
                    .Where(x => AnalyzedWeapons.Contains(x.Weapon));

                var bursts = burstHelper.DivideIntoBursts(weaponFireds, data.MatchStats, SINGLE_BURST_TOLERANCE)
                    .Where(x => x.WeaponFireds.Count >= MIN_SHOTS).ToList();

                // create misplays from bursts that fulfill the specified conditions
                var misplays = new List<RifleFiredWhileMoving>();
                foreach (var burst in bursts)
                {
                    // remove WeaponFireds that happened after the last enemy died
                    var allEnemiesDiedTime = data.LastPlayerOfTeamDied(burst.Round, burst.WeaponFireds.First().IsCt);
                    if(allEnemiesDiedTime != null)
                    {
                        burst.WeaponFireds = burst.WeaponFireds.Where(x => x.Time <= (int) allEnemiesDiedTime).ToList();
                    }

                    if (REQUIRE_NOT_FLASHED && data.GetFlasheds(burst.PlayerId, burst.Round, startTime: burst.StartTime, endTime: burst.StartTime).Any())
                        continue;

                    if (burst.WeaponFireds.Count < MIN_SHOTS)
                        continue;

                    // Apply INACCURATE_SHOTS condition(s)
                    var maxVelocityToCountAsAccurate = burst.EquipmentInfo.MaxPlayerSpeed * MIN_SPEED_FRACTION_TO_COUNT_AS_INACCURATE;
                    var inaccurateBullets = burst.WeaponFireds.Count(x => x.PlayerVelo.Length() > maxVelocityToCountAsAccurate);
                    if(inaccurateBullets < INACCURATE_SHOTS_MIN_BULLETS)
                    {
                        if ((double)inaccurateBullets / burst.WeaponFireds.Count < INACCURATE_SHOTS_MIN_FRACTION)
                            continue;
                    }

                    // Apply MAX_TIME_BEFORE_FIGHT by ignoring bursts where the player took place in no time during or after the burst
                    var fightTimeFrameStart = burst.WeaponFireds.First().Time;
                    var fightTimeFrameEnd = burst.WeaponFireds.Last().Time + MAX_TIME_BEFORE_FIGHT;

                    var firstDamageTaken = data.DamageTakens(burst.PlayerId, startTime: fightTimeFrameStart, endTime: fightTimeFrameEnd, requireEnemyDamage: true)
                        .FirstOrDefault();
                    var firstDamageDealt = data.DamageDealts(burst.PlayerId, startTime: fightTimeFrameStart, endTime: fightTimeFrameEnd, requireEnemyDamage: true)
                        .FirstOrDefault();
                    if (firstDamageTaken == null && firstDamageDealt == null) 
                    {
                        continue;
                    }

                    float enemyDistance = 0;
                    if (firstDamageTaken != null)
                        enemyDistance = (firstDamageTaken.PlayerPos - firstDamageTaken.VictimPos).Length();
                    else if (firstDamageDealt != null)
                        enemyDistance = (firstDamageDealt.PlayerPos - firstDamageDealt.VictimPos).Length();

                    if (enemyDistance < UnitConverter.MetersToUnits(MIN_ENEMY_DISTANCE))
                        continue;



                    if(MIN_DISTANCE_FROM_SMOKE_CENTER != -1)
                    {
                        var firstWeaponFired = burst.WeaponFireds.First();

                        var smokeInFrontConditionFulfilled = false;
                        foreach (var smoke in data.SmokeList.Where(x => x.Round == burst.Round))
                        {
                            var playerAimedOnSmoke = smoke.PlayerAimsAtSmoke(firstWeaponFired.PlayerPos, firstWeaponFired.PlayerView, firstWeaponFired.Time, firstWeaponFired.IsDucking);
                            if(playerAimedOnSmoke && (smoke.DetonationPos - firstWeaponFired.PlayerPos).Length() < UnitConverter.MetersToUnits(MIN_DISTANCE_FROM_SMOKE_CENTER))
                            {
                                smokeInFrontConditionFulfilled = true;
                                break;
                            }
                        }

                        if (smokeInFrontConditionFulfilled)
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
