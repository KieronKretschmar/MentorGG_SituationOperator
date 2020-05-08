using MatchEntities;
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
        SituationType SituationType { get; }

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        Task ClearTableAsync(SituationContext context, long matchId);

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        Task AnalyzeAndUploadAsync(SituationContext context, MatchDataSet matchData);
    }

    public abstract class SituationManager<TSituation> : ISituationManager where TSituation : class, ISituation
    {
        public abstract SituationType SituationType { get; }

        /// <summary>
        /// The detector for extracting occurences of TSituation.
        /// </summary>
        protected abstract IPatternDetector<TSituation> Detector { get; }

        /// <summary>
        /// Selects the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        protected abstract Func<SituationContext, DbSet<TSituation>> TableSelector { get; }

        public async Task AnalyzeAndUploadAsync(SituationContext context, MatchDataSet matchData)
        {
            var situations = await Detector.ExtractSituations(matchData);
            var table = TableSelector(context);
            table.AddRange(situations);
            await context.SaveChangesAsync();
        }

        public async Task ClearTableAsync(SituationContext context, long matchId)
        {
            var table = TableSelector(context);
            var existingEntries = table.Where(x => x.MatchId == matchId);
            table.RemoveRange(existingEntries);
            await context.SaveChangesAsync();
        }
    }
}
