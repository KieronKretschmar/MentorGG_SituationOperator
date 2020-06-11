using EquipmentLib;
using MatchEntities;
using MatchEntities.Interfaces;
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
    public class PushBeforeSmokeDetonatedManager : SinglePlayerSituationManager<PushBeforeSmokeDetonated>
    {
        /// <summary>
        /// Whether the player needs to die, as opposed to just taking damage, to count as a misplay.
        /// </summary>
        private const bool REQUIRE_DEATH = false;

        /// <summary>
        /// Maximum elapsed time in which the smoke must have detonated after taking damage to count as a misplay.
        /// </summary>
        private const int MAX_TIME_BEFORE_SMOKE_DETONATES = 800;

        /// <summary>
        /// Whether the smoke must have been thrown by a member of the player's team to count as a misplay.
        /// 
        /// Set value to -1 to ignore this condition.
        /// </summary>
        private const bool REQUIRE_TEAM_THROWER = true;

        private readonly IServiceProvider _sp;
        private readonly ILogger<PushBeforeSmokeDetonatedManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PushBeforeSmokeDetonatedManager(IServiceProvider sp, ILogger<PushBeforeSmokeDetonatedManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Movement;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.PushBeforeSmokeDetonated;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<PushBeforeSmokeDetonated>> TableSelector => context => context.PushBeforeSmokeDetonated;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<PushBeforeSmokeDetonated>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var equipmentHelper = _sp.GetRequiredService<IEquipmentHelper>();

                var misplays = new List<PushBeforeSmokeDetonated>();
                foreach (var smoke in data.SmokeList)
                {
                    var detonationTime = smoke.GetDetonationTime();

                    // Get all damages that were dealt right before the smoke detonated
                    var damages = data.DamageList
                        .Where(x => detonationTime - MAX_TIME_BEFORE_SMOKE_DETONATES <= x.Time && x.Time < detonationTime);

                    // Apply REQUIRE_DEATH condition
                    if (REQUIRE_DEATH)
                    {
                        damages = damages.Where(x => x.Fatal);
                    }

                    // Ignore damages not done by bullets (e.g. grenades)
                    damages = damages
                        .Where(x => equipmentHelper.GetEquipmentInfo(x.Weapon, data.MatchStats)?.IsFireArm() ?? false);

                    // Ignore consecutive damages taken by the same victim
                    damages = damages
                        .GroupBy(x => x.VictimId)
                        .Select(x => x.First());

                    foreach (var damage in damages)
                    {
                        // Apply REQUIRE_TEAM_THROWER condition
                        var victimIsCt = damage.IsCt == damage.TeamAttack;
                        if (REQUIRE_TEAM_THROWER && victimIsCt != smoke.IsCt)
                            continue;

                        // Ignore if the smoke would not have blocked the bullets trajectory
                        if (!smoke.BlocksLineOfSight(damage.PlayerPos, damage.VictimPos))
                        {
                            continue;
                        }

                        misplays.Add(new PushBeforeSmokeDetonated(smoke, damage));
                    }
                }

                return misplays;

            }
        }
    }
}
