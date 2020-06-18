using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Enums
{

    /// <summary>
    /// SubscriptionType - Master is located in MentorInterface
    /// </summary>
    public enum SubscriptionType : byte
    {
        Free = 1,
        Premium = 2,
        Ultimate = 3,

        /// <summary>
        /// Users who have MENTOR.GG in their name.
        /// </summary>
        Influencer = 4,
    }
}
