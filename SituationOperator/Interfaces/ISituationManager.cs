﻿using MatchEntities;
using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationOperator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperator
{
    /// <summary>
    /// Composition of required objects for the management of Situations that follow a specific pattern and are stored in a particular table in the database.
    /// </summary>
    public interface ISituationManager
    {
        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        SituationCategory SituationCategory { get; }

        /// <summary>
        /// Extracts situations from the data and uploads it to the database.
        /// </summary>
        Task AnalyzeAndUploadAsync(SituationContext context, MatchDataSet matchData);

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        Task ClearTableAsync(SituationContext context, long matchId);
    }

    /// <summary>
    /// Abstract base for all SituationManagers
    /// </summary>
    /// <typeparam name="TSituation">The type of Situation</typeparam>
    public abstract class SituationManager<TSituation> : ISituationManager where TSituation : class, ISituation
    {
        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        public abstract SituationCategory SituationCategory { get; }

        /// <summary>
        /// The detector for extracting occurences of TSituation.
        /// </summary>
        protected abstract IPatternDetector<TSituation> Detector { get; }

        /// <summary>
        /// Selects the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        protected abstract Func<SituationContext, DbSet<TSituation>> TableSelector { get; }

        /// <summary>
        /// Extracts situations from the data and uploads it to the database.
        /// </summary>
        public async Task AnalyzeAndUploadAsync(SituationContext context, MatchDataSet matchData)
        {
            var situations = await Detector.ExtractSituations(matchData);
            var table = TableSelector(context);
            table.AddRange(situations);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        public async Task ClearTableAsync(SituationContext context, long matchId)
        {
            var table = TableSelector(context);
            var existingEntries = table.Where(x => x.MatchId == matchId);
            table.RemoveRange(existingEntries);
            await context.SaveChangesAsync();
        }
    }
}
