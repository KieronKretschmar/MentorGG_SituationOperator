﻿using EquipmentLib;
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

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class MissedTradeKillManager : SinglePlayerSituationManager<MissedTradeKill>
    {
        /// <summary>
        /// Minimium distance in meters between the player and the teammate at the point of the latter's death to count as a misplay.
        /// </summary>
        private const double MAX_DISTANCE_TO_TEAMMATE = 4;

        /// <summary>
        /// Minimium time the teammates' killer must have survived after the kill to count as a misplay.
        /// </summary>
        private const int MIN_TIME_KILLER_SURVIVED = 3141;

        /// <summary>
        /// Minimium time between the teammates' death and the player dealing or taking damage to count as a misplay.
        /// </summary>
        private const int MIN_TIME_NOT_FIGHTING = 3141;

        /// <summary>
        /// Maximum distance in meters between the player and the killer if the killer had a sniper and and the player did not to count as a misplay.
        /// </summary>
        private const double MAX_DISTANCE_TO_SNIPER_KILLER = 25;

        private readonly IServiceProvider _sp;
        private readonly ILogger<MissedTradeKillManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MissedTradeKillManager(IServiceProvider sp, ILogger<MissedTradeKillManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Tactical;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.MissedTradeKill;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<MissedTradeKill>> TableSelector => context => context.MissedTradeKill;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<MissedTradeKill>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var misplays = new List<MissedTradeKill>();
                foreach (var kill in data.KillList)
                {
                    if (kill.TeamKill == true)
                        continue;

                    // ignore grenadekills
                    if (kill.Weapon.IsGun() == false)
                        continue;

                    // ignore long-distance kills with AWP, as they're hard to trade
                    if (kill.Weapon.IsSniperRifle() && kill.Distance() > MAX_DISTANCE_TO_SNIPER_KILLER)
                    {
                        continue;
                    }

                    // ignore if the killer died shortly after
                    var killerDeath = data.Death(kill.PlayerId, kill.Round, kill.Time);
                    if (killerDeath != null && killerDeath.Time < kill.Time + MIN_TIME_KILLER_SURVIVED)
                        continue;

                    var aliveTeammates = data.GetTeamRoundStats(kill.VictimId, kill.Round)
                        .Where(x => x.PlayerId != kill.VictimId)
                        .Where(x=>data.IsAlive(x.PlayerId, x.Round, kill.Time));
                    foreach (var player in aliveTeammates)
                    {
                        var distanceToVictim = data.LastPositionDistance(kill.VictimId, player.PlayerId, kill.Time);
                        if (distanceToVictim == null || distanceToVictim > UnitConverter.MetersToUnits(MAX_DISTANCE_TO_TEAMMATE))
                            continue;

                        var playerWasFighting = data.PlayerDealtOrTookDamage(player.PlayerId, kill.Round, startTime: kill.Time, endTime: kill.Time + MIN_TIME_NOT_FIGHTING);
                        if (playerWasFighting)
                            continue;

                        misplays.Add(new MissedTradeKill(player.PlayerId, kill, (int) distanceToVictim));
                    }
                }

                return misplays;
            }
        }
    }
}
