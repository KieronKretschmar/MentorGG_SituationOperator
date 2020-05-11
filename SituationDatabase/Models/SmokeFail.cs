using MatchEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SituationDatabase.Models
{
    public class SmokeFail : SinglePlayerSituation, ISinglePlayerSituation
    {
        public SmokeFail(Smoke smoke) : base(smoke.MatchId, smoke.Round, TrajectoryHelper.GetThrowTime(smoke), smoke.PlayerId)
        {
            LineupId = smoke.LineUp;

            //TODO: Set LineupName, e.g. in SituationManager.EnrichData()
        }

        public long SteamId { get; set; }
        public int LineupId { get; set; }
        public string LineupName { get; set; }
    }
}
