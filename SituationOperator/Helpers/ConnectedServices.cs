using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// A collection of connected services
    /// </summary>
    public static class ConnectedServices
    {
        /// <summary>
        /// Match Retriever
        /// </summary>
        public static ConnectedService MatchRetriever = new ConnectedService(
            "match-retriever",
            "match-retriever");
    }


    /// <summary>
    /// A Connected Service that is communicated via MentorInterface.
    /// </summary>
    public struct ConnectedService
    {
        /// <summary>
        /// Human readable name of the service.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// DNS Address for internal communication.
        /// </summary>
        public readonly string DNSAddress;

        /// <summary>
        /// Whether to use Https instead of Http.
        /// </summary>
        public readonly bool UseHttps;

        /// <summary>
        /// Define a Connected Service.
        /// </summary>
        /// <param name="name">Name of the service</param>
        /// <param name="dnsAddress">DNS Address of the service</param>
        public ConnectedService(string name, string dnsAddress, bool useHttps = false)
        {
            Name = name;
            DNSAddress = dnsAddress;
            UseHttps = useHttps;
        }

        /// <summary>
        /// Return the ConnectedService's name
        /// </summary>
        public static implicit operator string(ConnectedService connectedService)
        {
            return connectedService.Name;
        }
    }
}
