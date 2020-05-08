using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.PatternDetectors;
using SituationOperator.PatternDetectors.Misplays;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers.Misplays
{
    /// <summary>
    /// Manager for failed smoke lineups.
    /// </summary>
    public class SmokeFailManager : SituationManager<SmokeFail>
    {
        private readonly SmokeFailDetector _detector;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="detector">Detector for this Situation</param>
        public SmokeFailManager(SmokeFailDetector detector)
        {
            _detector = detector;
        }

        /// <inheritdoc/>
        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        /// <inheritdoc/>
        protected override IPatternDetector<SmokeFail> Detector => _detector;

        /// <inheritdoc/>
        protected override Func<SituationContext, DbSet<SmokeFail>> TableSelector => context => context.SmokeFail;
    }
}
