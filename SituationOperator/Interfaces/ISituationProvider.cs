using MatchEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationOperator
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISituationProvider<T> where T : IAction
    {
        IEnumerable<IAction> ComputeActions(MatchStats matchStats);
        IEnumerable<IAction> LoadActionsFromDatabase(long matchId);

    }
}
