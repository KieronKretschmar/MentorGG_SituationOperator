using MatchEntities;
using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers
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
        Task AnalyzeAndUploadAsync(MatchDataSet matchData);

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        Task ClearTableAsync(long matchId);

        Task<List<ISituation>> LoadSituations(long matchId);
    }

    /// <summary>
    /// Abstract base for all SituationManagers
    /// </summary>
    /// <typeparam name="TSituation">The type of Situation</typeparam>
    public abstract class SituationManager<TSituation> : ISituationManager where TSituation : class, ISituation
    {
        private readonly SituationContext _context;

        public SituationManager(SituationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        public abstract SituationCategory SituationCategory { get; }

        /// <summary>
        /// Selects the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        protected abstract Func<SituationContext, DbSet<TSituation>> TableSelector { get; }

        /// <summary>
        /// Extracts situations from the data and uploads it to the database.
        /// </summary>
        public async Task AnalyzeAndUploadAsync(MatchDataSet matchData)
        {
            var situations = await ExtractSituationsAsync(matchData);
            var table = TableSelector(_context);
            table.AddRange(situations);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        public async Task ClearTableAsync(long matchId)
        {
            var table = TableSelector(_context);
            var existingEntries = table.Where(x => x.MatchId == matchId);
            table.RemoveRange(existingEntries);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ISituation>> LoadSituations(long matchId)
        {
            var table = TableSelector(_context);
            var existingEntries = table.Where(x => x.MatchId == matchId);
            var res = await existingEntries.Select(x => x as ISituation).ToListAsync();
            return res;
        }

        /// <summary>
        /// Returns situations that implement the specific pattern.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task<IEnumerable<TSituation>> ExtractSituationsAsync(MatchDataSet data);
    }
}
