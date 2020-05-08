﻿using Microsoft.EntityFrameworkCore;
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
        public override SituationType SituationType => SituationType.Misplay;

        protected override IPatternDetector<SmokeFail> Detector => new SmokeFailDetector();

        protected override Func<SituationContext, DbSet<SmokeFail>> TableSelector => context => context.SmokeFail;
    }
}
