using MatchEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// Manager for HE grenades that dealt a lot of damage.
    /// </summary>
    public class EffectiveHeGrenadeManager : SituationManager<EffectiveHeGrenade>
    {
        private const int MIN_TOTAL_DAMAGE = 50;

        private readonly IServiceProvider _sp;
        private readonly ILogger<EffectiveHeGrenadeManager> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        public EffectiveHeGrenadeManager(IServiceProvider sp, ILogger<EffectiveHeGrenadeManager> logger, SituationContext context) : base(context)
        {
            _sp = sp;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Goodplay;


        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<EffectiveHeGrenade>> TableSelector => context => context.EffectiveHeGrenade;

        /// <summary>
        /// Finds all HE grenades that were effective against enemies.
        /// </summary>
        /// <param name="data">Data of the match in which to look for situations for all players.</param>
        /// <returns></returns>
        protected override async Task<IEnumerable<EffectiveHeGrenade>> ExtractSituationsAsync(MatchDataSet data)
        {
            var hes = data.HeList
                .Select(x=> new EffectiveHeGrenade(x, data.DamagesByHe(x)))
                .Where(x => x.TotalEnemyDamage > MIN_TOTAL_DAMAGE || x.EnemiesKilled > 0);

            return hes;
        }
    }
}
