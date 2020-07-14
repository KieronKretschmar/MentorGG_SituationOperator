using SituationDatabase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Models
{
    public class FeedbackModel
    {
        public List<UserFeedback> UserFeedbacks { get; set; }

        public FeedbackModel(List<UserFeedback> userFeedbacks)
        {
            UserFeedbacks = userFeedbacks;
        }
    }
}
