using RabbitCommunicationLib.TransferModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator
{
    public class SituationOperatorResponseModel : TransferModel
    {
        public SituationOperatorResult Status { get; set; }
    }
}
