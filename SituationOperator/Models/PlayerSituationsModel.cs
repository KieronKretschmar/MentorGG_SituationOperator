using SituationDatabase.Models;
using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    public partial class PlayerSituationsModel
    {
        public Dictionary<SituationType, SituationCollection> Misplays { get; set; }
        public Dictionary<SituationType, SituationCollection> Highlights { get; set; }

        /// <summary>
        /// Contains metadata about the analyzed matches
        /// </summary>
        public Dictionary<long,MatchInfo> Matches { get; set; }

        public PlayerSituationsModel()
        {
            Misplays = new Dictionary<SituationType, SituationCollection>();
            Highlights = new Dictionary<SituationType, SituationCollection>();
            Matches = new Dictionary<long, MatchInfo>();
        }
    }
}
