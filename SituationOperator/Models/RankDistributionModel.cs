using SituationDatabase;
using SituationDatabase.Enums;
using SituationDatabase.Models;
using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    /// <summary>
    /// Holds data about how <see cref="ISituation"/>s of different <see cref="SituationType"/> are distributed among different ranks.
    /// </summary>
    public class RankDistributionModel
    {
        /// <summary>
        /// Dictionary with data for each <see cref="SituationType"/>.
        /// </summary>
        public Dictionary<SituationType, Dictionary<int, SituationInfoByRank>> Data { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RankDistributionModel()
        {
            Data = new Dictionary<SituationType, Dictionary<int, SituationInfoByRank>>();
        }

    }
}
