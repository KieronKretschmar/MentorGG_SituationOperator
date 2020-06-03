using SituationOperator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers.SubscriptionConfig
{

    /// <summary>
    /// A Configuration model representing the SubscriptionConfiguration object to be mounted
    /// inside the container by Kubernetes.
    /// </summary>
    public class SubscriptionConfig
    {
        public SubscriptionSettings Free { get; set; }

        public SubscriptionSettings Premium { get; set; }

        public SubscriptionSettings Ultimate { get; set; }

        /// <summary>
        /// Return the corresponding SubscriptionSettings for a SubscriptionType
        /// </summary>
        public SubscriptionSettings SettingsFromSubscriptionType(SubscriptionType subscriptionType)
        {
            switch (subscriptionType)
            {
                case SubscriptionType.Free:
                    return Free;

                case SubscriptionType.Premium:
                    return Premium;

                case SubscriptionType.Ultimate:
                    return Ultimate;

                default:
                    throw new InvalidOperationException("Unknown SubscriptionType!");
            }
        }

    }

    public class SubscriptionSettings
    {
        public int MatchAccessDurationInDays { get; set; }

        public int DailyMatchesLimit { get; set; }
    }
}
