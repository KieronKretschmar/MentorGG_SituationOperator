using Microsoft.EntityFrameworkCore;
using SituationDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.SituationManagers
{
    /// <summary>
    /// An <see cref="ISituationManager"/> for managing a SituationType that implements <see cref="ISinglePlayerSituation"/>.
    /// </summary>
    public interface ISinglePlayerSituationManager : ISituationManager
    {

        /// <summary>
        /// Loads all Situations managed by this manager of the specified player in the given matches.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="matchIds"></param>
        /// <returns></returns>
        Task<List<ISinglePlayerSituation>> LoadSituationsAsync(long steamId, List<long> matchIds);
    }

    /// <summary>
    /// Abstract base for all SinglePlayer SituationManagers.
    /// </summary>
    /// <typeparam name="TSituation">The type of Situation</typeparam>
    public abstract class SinglePlayerSituationManager<TSituation> : SituationManager<TSituation>, ISinglePlayerSituationManager
        where TSituation : class, ISinglePlayerSituation
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public SinglePlayerSituationManager(SituationContext context) : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<List<ISinglePlayerSituation>> LoadSituationsAsync(long steamId, List<long> matchIds)
        {
            var table = TableSelector(_context);
            var existingEntries = table.Where(x => x.SteamId == steamId && matchIds.Contains(x.MatchId));
            var res = await existingEntries.Select(x => x as ISinglePlayerSituation).ToListAsync();
            return res;
        }
    }
}
