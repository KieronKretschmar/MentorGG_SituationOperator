using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SituationDatabase;

namespace SituationOperator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Migrate the Database if it's NOT an InMemory Database.
            // (╯°□°）╯︵ ┻━┻
            using (var scope = host.Services.CreateScope())
            {
                if (scope.ServiceProvider.GetRequiredService<SituationContext>().Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    scope.ServiceProvider.GetRequiredService<SituationContext>().Database.Migrate();
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(
                        "http://*:80",
                        $"http://*:{Startup.METRICS_PORT}");

                    webBuilder.UseStartup<Startup>();
                });
    }
}
