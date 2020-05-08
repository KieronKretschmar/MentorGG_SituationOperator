using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.PatternDetectors;
using SituationOperator.PatternDetectors.Goodplays;
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
        private readonly EffectiveHeGrenadeDetector _detector;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="detector">Detector for this Situation</param>
        public EffectiveHeGrenadeManager(EffectiveHeGrenadeDetector detector)
        {
            _detector = detector;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Goodplay;

        /// <inheritdoc/>
        protected override IPatternDetector<EffectiveHeGrenade> Detector => _detector;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<EffectiveHeGrenade>> TableSelector => context => context.EffectiveHeGrenade;
    }
}
