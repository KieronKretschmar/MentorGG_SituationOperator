using MatchEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace SituationOperator.SituationManagers.Misplays
{
    /// <summary>
    /// Manager for failed smoke lineups.
    /// </summary>
    public class SmokeFailManager : SituationManager<SmokeFail>
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
        protected override Func<SituationContext, DbSet<SmokeFail>> TableSelector => context => context.SmokeFail;


        /// <summary>
        /// Returns all failed smoke attempts of known lineups.
        /// </summary>
        /// <param name="data">Data of the match in which to look for situations for all players.</param>
        /// <returns></returns>
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
                    .Select(x => new SmokeFail
                    {
                        MatchId = x.MatchId,
                        Map = x.MatchStats.Map,
                        MatchDate = x.MatchStats.MatchDate,
                        Round = x.Round,
                        StartTime = x.Time,
                        PlayerId = x.PlayerId,
                        LineupId = x.LineUp,
                        LineupName = lineups[x.LineUp].Name
                    })
                    .ToList();

                return failedSmokes;
            }
        }
    }
}
