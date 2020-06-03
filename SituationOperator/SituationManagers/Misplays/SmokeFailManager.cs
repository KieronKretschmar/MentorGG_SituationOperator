using MatchEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
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
    public class SmokeFailManager : SinglePlayerSituationManager<SmokeFail>
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<SmokeFailManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SmokeFailManager(IServiceProvider sp, ILogger<SmokeFailManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        public override SkillDomain SkillDomain => SkillDomain.Grenades;

        /// <inheritdoc/>
        public override SituationType SituationType => SituationType.SmokeFail;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<SmokeFail>> TableSelector => context => context.SmokeFail;

        /// <inheritdoc/>
        protected override async Task<IEnumerable<SmokeFail>> ExtractSituationsAsync(MatchDataSet data)
        {
            using (var scope = _sp.CreateScope())
            {
                var zoneReader = scope.ServiceProvider.GetRequiredService<IZoneReader>();

                var map = data.MatchStats.Map;

                // If the map is not known, return empty collection
                if (!Enum.TryParse(map, true, out ZoneReader.Enums.Map mapFromEnum))
                {
                    _logger.LogDebug($"No zones are defined for map {map} and match {data.MatchId}, returning empty collection");
                    return new List<SmokeFail>();
                }

                var lineups = zoneReader.GetLineups(ZoneReader.Enums.LineupType.Smoke, mapFromEnum).Lineups;

                var failedSmokes = data.SmokeList
                    .Where(x => x.Result == MatchEntities.Enums.TargetResult.Miss)
                    .Select(x => new SmokeFail(x))
                    .ToList();

                return failedSmokes;
            }
        }
    }
}
