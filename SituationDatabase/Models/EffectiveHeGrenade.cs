using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase.Models
{
    public class EffectiveHeGrenade : Situation, ISinglePlayerSituation
    {
        public long SteamId { get; set; }
        public int LineupId { get; set; }
        public string LineupName { get; set; }
    }
}
