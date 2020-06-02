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

        public PlayerSituationsModel()
        {
            Misplays = new Dictionary<SituationType, SituationCollection>();
            Highlights = new Dictionary<SituationType, SituationCollection>();
        }
    }
}
