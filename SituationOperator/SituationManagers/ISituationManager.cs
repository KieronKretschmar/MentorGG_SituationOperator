﻿using MatchEntities;
using MatchEntities.Enums;
using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using SituationOperator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// Identifies the SituationCategory the managed situation belongs to.
        /// </summary>
        SituationCategory SituationCategory { get; }

        /// <summary>
        /// Identifies the SkillDomain of this SituationType.
        /// </summary>
        SkillDomain SkillDomain { get; }

        /// <summary>
        /// Identifies the type of situation.
        /// </summary>
        SituationType SituationType { get; }

        /// <summary>
        /// Extracts situations from the data and uploads them to the database.
        /// </summary>
        Task AnalyzeAndUploadAsync(MatchDataSet matchData);

        /// <summary>
        /// Clears the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        Task ClearTableAsync(long matchId);

        /// <summary>
        /// Loads a SituationCollection with all Situations managed by this manager of the given match.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task<SituationCollection> LoadSituationCollectionAsync(long matchId);

        /// <summary>
        /// Loads all Situations managed by this manager of the given match.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task<List<ISituation>> LoadSituationsAsync(long matchId);

        /// <summary>
        /// Loads a SituationCollection with all Situations managed by this manager of the given matches.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        Task<SituationCollection> LoadSituationCollectionAsync(List<long> matchIds);

        /// <summary>
        /// Loads all Situations managed by this manager of the given matches.
        /// <returns></returns>
        /// </summary>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        Task<List<ISituation>> LoadSituationsAsync(List<long> matchIds);

        /// <summary>
        /// Returns Dictionary containing info about occurences of situations for different ranks with the (floored) rank as key.
        /// </summary>
        /// <param name="minAnalysisDate"></param>
        /// <param name="maxAnalysisDate"></param>
        /// <returns></returns>
        Task<Dictionary<int, SituationInfoByRank>> GetDistribution(long? minMatchId = null, long? maxMatchId = null);
    }

    /// <summary>
    /// Abstract base for all SituationManagers.
    /// </summary>
    /// <typeparam name="TSituation">The type of Situation</typeparam>
    public abstract class SituationManager<TSituation> : ISituationManager where TSituation : class, ISituation
    {
        protected readonly SituationContext _context;

        public SituationManager(SituationContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public abstract SituationCategory SituationCategory { get; }

        /// <inheritdoc/>
        public abstract SkillDomain SkillDomain { get; }

        /// <inheritdoc/>
        public abstract SituationType SituationType { get; }

        /// <summary>
        /// Selects the table of the SituationDatabase in which occurences of TSituation are stored.
        /// </summary>
        protected abstract Func<SituationContext, DbSet<TSituation>> TableSelector { get; }

        /// <inheritdoc/>
        public async Task AnalyzeAndUploadAsync(MatchDataSet matchData)
        {
            var situations = await ExtractSituationsAsync(matchData);
            var table = TableSelector(_context);
            table.AddRange(situations);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task ClearTableAsync(long matchId)
        {
            var table = TableSelector(_context);
            var existingEntries = table.Where(x => x.MatchId == matchId);
            table.RemoveRange(existingEntries);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<SituationCollection> LoadSituationCollectionAsync(long matchId)
        {
            var situations = await LoadSituationsAsync(matchId);
            var situationCollection = new SituationCollection(SituationType, SkillDomain, situations);
            return situationCollection;
        }

        /// <inheritdoc/>
        public async Task<List<ISituation>> LoadSituationsAsync(long matchId)
        {
            var table = TableSelector(_context);
            var existingEntries = table.Where(x => x.MatchId == matchId);
            var res = await existingEntries.Select(x => x as ISituation).ToListAsync();
            return res;
        }

        /// <inheritdoc/>
        public async Task<SituationCollection> LoadSituationCollectionAsync(List<long> matchIds)
        {
            var situations = await LoadSituationsAsync(matchIds);
            var situationCollection = new SituationCollection(SituationType, SkillDomain, situations);
            return situationCollection;
        }

        /// <inheritdoc/>
        public async Task<List<ISituation>> LoadSituationsAsync(List<long> matchIds)
        {
            var table = TableSelector(_context);
            var existingEntries = table.Where(x => matchIds.Contains(x.MatchId));
            var res = await existingEntries.Select(x => x as ISituation).ToListAsync();
            return res;
        }

        /// <inheritdoc/>
        public async Task<Dictionary<int, SituationInfoByRank>> GetDistribution(long? minMatchId = null, long? maxMatchId = null)
        {
            var table = TableSelector(_context);

            // Set minAnalysisDate to at least the matchdate from which on these situations were analyzed
            var firstAnalyzedMatchId = await GetFirstAnalyzedMatchId();
            minMatchId = Math.Max(minMatchId ?? 0, firstAnalyzedMatchId ?? 0);

            var dateCondition = $" AND {minMatchId} <= playermatchstats.MatchId ";

            if (maxMatchId != null)
            {
                dateCondition += $" AND playermatchstats.MatchId < {maxMatchId} ";
            }

            // Notes:
            // - For PlayerRoundCount, each round of the match is counted once for every relevant player, even if he left the game for a few rounds
            var mysql =
                $"  SELECT SituationCountsByRank.RankBeforeMatch, PlayerRoundCount, SituationCount FROM " +
                //  table SituationCountsByRank containing Ranks with corresponding SituationCounts
                $"  ( " +
                $"      SELECT RankBeforeMatch, COUNT(RankBeforeMatch) AS SituationCount FROM " +
                //      table AllSituationsTable containing Ranks with corresponding SituationCounts
                $"      ( " +
                $"          SELECT playermatchstats.RankBeforeMatch, playermatchstats.SteamId FROM  " +
                $"          situationoperator.{SituationType.ToString().ToLowerInvariant()} " +
                $"          INNER JOIN matchdata.playermatchstats " +
                $"          ON {SituationType.ToString().ToLowerInvariant()}.MatchId = playermatchstats.MatchId AND {SituationType.ToString().ToLowerInvariant()}.SteamId = playermatchstats.SteamId " +
                $"          WHERE playermatchstats.SteamId > 0 " +
                $"          {dateCondition} " +
                $"      ) AS AllSituationsTable " +
                $"      GROUP BY RankBeforeMatch " +
                $"  ) AS SituationCountsByRank " +
                $"  INNER JOIN " +
                //  table RoundCountsByRank containing Ranks with corresponding RoundCounts
                $"  ( " +
                $"      SELECT RankBeforeMatch, SUM(PlayerRoundCount) AS PlayerRoundCount FROM " +
                //      table RoundCountTable containing all PlayerMatches with Ranks and RoundCounts
                $"      ( " +
                $"          SELECT match.MatchId, playermatchstats.SteamId, playermatchstats.RankBeforeMatch, match.Rounds AS PlayerRoundCount FROM situationoperator.match " +
                $"          INNER JOIN matchdata.playermatchstats " +
                $"          ON match.MatchId = playermatchstats.MatchId " +
                $"          WHERE playermatchstats.SteamId > 0 " +
                $"          {dateCondition} " +
                $"      ) AS RoundCountTable " +
                $"      GROUP BY RankBeforeMatch " +
                $"  ) AS RoundCountsByRank " +
                $"  ON SituationCountsByRank.RankBeforeMatch = RoundCountsByRank.RankBeforeMatch ";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            var res = await _context.RankDistribution.FromSqlRaw(mysql).ToDictionaryAsync(x => x.RankBeforeMatch, x => x);
            sw.Stop();

            return res;
        }

        private async Task<long?> GetFirstAnalyzedMatchId()
        {
            var table = TableSelector(_context);
            var firstAnalyzedMatchId = await table
                .Select(x => x.MatchId)
                .Distinct()
                .OrderBy(x=>x)
                .FirstOrDefaultAsync();

            return firstAnalyzedMatchId;
        }

        /// <summary>
        /// Returns situations that implement the specific pattern.
        /// 
        /// For more info see the issue linked in the XML comment of the TSituation.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract Task<IEnumerable<TSituation>> ExtractSituationsAsync(MatchDataSet data);
    }
}
