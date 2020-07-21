using MatchEntities;
using Microsoft.EntityFrameworkCore;
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

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// A SituationManager. 
    /// See <see cref="ExtractSituationsAsync(MatchDataSet)"/> for more info regarding Situation specific logic.
    /// </summary>
    public class ClutchManager : SinglePlayerSituationManager<Clutch>
    {
        /// <summary>
        /// Assuming the clutcher is Terrorist and the bomb was planted, this is the minimum amount of time which the bomb would need to explode at clutch-start to count as a highlight.
        /// </summary>
        private const int MIN_TIME_BOMB_TIME_TICKING_LEFT = 12000;

        private readonly IServiceProvider _sp;
        private readonly ILogger<ClutchManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClutchManager(IServiceProvider sp, ILogger<ClutchManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Highlight;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Shooting;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.Clutch;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<Clutch>> TableSelector => context => context.Clutch;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<Clutch>> ExtractSituationsAsync(MatchDataSet data)
        {
            var highlights = new List<Clutch>();
            foreach (var round in data.RoundStatsList)
            {
                var winningPlayers = data.GetPlayerRoundStatsByRoundOutcome(round.Round, true);
                var winningPlayerDeaths = winningPlayers.ToDictionary(
                    x => x,
                    x => data.Death(x.PlayerId, round.Round))
                    .Where(x=>x.Value != null)
                    .ToDictionary(x=>x.Key, x=>x.Value);

                var winningSurvivors = winningPlayers
                    .Where(x => winningPlayerDeaths.ContainsKey(x) == false)
                    .ToList();

                // Continue if more than 1 winner survived
                if (winningSurvivors.Count > 1)
                    continue;

                // The potential clutcher is either the only survivor or the last one to die
                var potentialClutcher = winningSurvivors.Count > 0
                    ? winningSurvivors.Single()
                    : winningPlayerDeaths.OrderByDescending(x => x.Value.Time).First().Key;

                var lastTeammatesDeath = winningPlayerDeaths
                    .Where(x => x.Key.PlayerId != potentialClutcher.PlayerId)
                    .OrderBy(x => x.Value.Time)
                    .Last().Value;

                var losingPlayers = data.GetPlayerRoundStatsByRoundOutcome(round.Round, false);

                var enemiesAlive = losingPlayers
                    .Where(x => data.IsAlive(x.PlayerId, round.Round, lastTeammatesDeath.Time))
                    .ToList();
                
                if (enemiesAlive.Count < 2)
                    continue;

                if(potentialClutcher.IsCt == false)
                {
                    // Apply MIN_TIME_BOMB_TIME_TICKING_LEFT condition
                    var bombPlant = data.BombPlantList.SingleOrDefault(x => x.Success && x.Round == round.Round);
                    if (bombPlant != null)
                    {
                        var tickingTimePassed = lastTeammatesDeath.Time - bombPlant.Time;
                        var timeTickingLeft = data.GetMatchSettings().C4Timer - tickingTimePassed;
                        if (timeTickingLeft < MIN_TIME_BOMB_TIME_TICKING_LEFT)
                            continue;
                    }
                }

                highlights.Add(new Clutch(round, potentialClutcher.PlayerId, lastTeammatesDeath.Time, enemiesAlive.Count()));
            }

            return highlights;
        }
    }
}
