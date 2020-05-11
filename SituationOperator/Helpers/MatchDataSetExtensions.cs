using MatchEntities;
using MatchEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    public static class MatchDataSetExtensions
    {
        /// <summary>
        /// Gets the RoundStats object referenced by the given entity.
        /// </summary>
        /// <param name="matchData"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static RoundStats RoundStats(this MatchDataSet matchData, IRoundEntity entity)
        {
            return matchData.RoundStatsList.Single(x => x.Round == entity.Round);
        }

        /// <summary>
        /// Gets the Damage entities dealt by a HE grenade.
        /// </summary>
        /// <param name="matchData"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<Damage> DamagesByHe(this MatchDataSet matchData, He entity)
        {
            return matchData.DamageList.Where(x => x.HeGrenadeId == entity.GrenadeId).ToList();
        }
    }
}
