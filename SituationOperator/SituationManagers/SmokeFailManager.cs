using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Interfaces;
using SituationOperator.PatternDetectors;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers
{
    public class SmokeFailManager : SituationManager<SmokeFail>
    {
        private readonly SmokeFailDetector _smokeFailDetector;

        public SmokeFailManager(SmokeFailDetector smokeFailDetector)
        {
            _smokeFailDetector = smokeFailDetector;
        }

        public override SituationCategory SituationCategory => SituationCategory.Misplay;

        protected override IPatternDetector<SmokeFail> Detector => _smokeFailDetector;

        protected override Func<SituationContext, DbSet<SmokeFail>> TableSelector => context => context.SmokeFail;
    }
}
