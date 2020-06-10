using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SituationOperator.Helpers
{
    /// <summary>
    /// Extensions for IServiceCollection
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the connectedService to the services collection, using the default url, or, 
        /// if provided, with the url specified in the environment with the overrideUrlKey.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectedService"></param>
        /// <param name="configuration"></param>
        /// <param name="overrideUrlKey"></param>
        public static void AddConnectedHttpService(this IServiceCollection services, ConnectedService connectedService, IConfiguration configuration, string overrideUrlKey = null)
        {
            var url = $"{(connectedService.UseHttps ? "https" : "http")}://{connectedService.DNSAddress}";
            if (configuration.GetValue<string>(overrideUrlKey) != null)
            {
                url = configuration.GetValue<string>(overrideUrlKey);
                Console.WriteLine(
                    $"Warning: {overrideUrlKey} [ {url} ] detected for {connectedService}. " +
                    $"Overriding default address."
                    );
            }
            services.AddHttpClient(connectedService, c =>
            {
                c.BaseAddress = new Uri(url);
                c.DefaultRequestHeaders.Add("User-Agent", "SituationOperator");
            });
        }
    }
}
