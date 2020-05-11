using MatchEntities;
using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers.GoodPlays
{
    /// <summary>
    /// Manager for HE grenades that dealt a lot of damage.
    /// </summary>
    public class EffectiveHeGrenadeManager : SituationManager<EffectiveHeGrenade>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="detector">Detector for this Situation</param>
        public EffectiveHeGrenadeManager()
        {
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Goodplay;


        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<EffectiveHeGrenade>> TableSelector => context => context.EffectiveHeGrenade;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">Data of the match in which to look for situations for all players.</param>
        /// <returns></returns>
        protected override Task<IEnumerable<EffectiveHeGrenade>> ExtractSituationsAsync(MatchDataSet data)
        {
            throw new NotImplementedException();
        }
    }
}
