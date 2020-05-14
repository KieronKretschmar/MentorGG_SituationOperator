using RabbitCommunicationLib.TransferModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Communications
{
    public class SituationOperatorResponseModel : TransferModel
    {
        /// <summary>
        /// Id of the Match which was attempted to be analyzed by SituationOperator.
        /// </summary>
        public long MatchId { get; set; }

        /// <summary>
        /// The outcome of the attemped analysis.
        /// </summary>
        public SituationOperatorResult Status { get; set; }
    }
}
