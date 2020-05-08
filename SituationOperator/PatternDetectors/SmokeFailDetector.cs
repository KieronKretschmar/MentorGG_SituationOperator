using MatchEntities;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Models;
using SituationOperator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZoneReader;

namespace SituationOperator.PatternDetectors
{
    public class SmokeFailDetector : IPatternDetector<SmokeFail>
    {
        private readonly ILogger<SmokeFailDetector> _logger;
        private readonly IZoneReader _zoneReader;

        public SmokeFailDetector(
            ILogger<SmokeFailDetector> logger,
            IZoneReader zoneReader)
        {
            _logger = logger;
            _zoneReader = zoneReader;
        }

        public async Task<IEnumerable<SmokeFail>> ExtractSituations(MatchDataSet matchData)
        {
            var map = matchData.MatchStats.Map;

            // If the map is not known, return empty collection
            if (!Enum.TryParse(map, true, out ZoneReader.Enums.Map mapFromEnum))
            {
                _logger.LogDebug($"No zones are defined for map {map} and match {matchData.MatchId}, returning empty collection");
                return new List<SmokeFail>();
            }

            var lineups = _zoneReader.GetLineups(ZoneReader.Enums.LineupType.Smoke, mapFromEnum).Lineups;

            var failedSmokes = matchData.SmokeList
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
