using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Prometheus;
using SituationOperator.Helpers;
using SituationDatabase;
using Microsoft.EntityFrameworkCore;
using Database;

namespace SituationOperator
{
    public class Startup
    {
        /// <summary>
        /// Port to scrape metrics from at `/metrics`
        /// </summary>
        public const int METRICS_PORT = 9913;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddApiVersioning();

            #region Logging
            services.AddLogging(o =>
            {
                o.AddConsole(o =>
                {
                    o.TimestampFormat = "[yyyy-MM-dd HH:mm:ss zzz] ";
                });

                o.AddConsole(options =>
                {
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss zzz] ";
                });
                o.AddDebug();

                // Filter out ASP.Net and EFCore logs of LogLevel lower than LogLevel.Warning
                o.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                o.AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);
                o.AddFilter("Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker", LogLevel.Warning);
                o.AddFilter("Microsoft.AspNetCore.Mvc.Infrastructure.ObjectResultExecutor", LogLevel.Warning);
                o.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Warning);
                o.AddFilter("Microsoft.AspNetCore.Routing.EndpointMiddleware", LogLevel.Warning);

                // Filter logs for each forwarded request
                o.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
            });
            #endregion

            #region Situation Database
            var MYSQL_CONNECTION_STRING = GetOptionalEnvironmentVariable<string>(Configuration, "MYSQL_CONNECTION_STRING", null);
            // if a connectionString is set use mysql, else use InMemory
            if (MYSQL_CONNECTION_STRING != null)
            {
                // Add context as Transient instead of Scoped, as Scoped lead to DI error and does not have advantages under non-http conditions
                services.AddDbContext<SituationContext>(o => { o.UseMySql(MYSQL_CONNECTION_STRING); }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            }
            else
            {
                Console.WriteLine("WARNING: Using InMemoryDatabase!");

                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<SituationContext>((sp, options) =>
                    {
                        options.UseInMemoryDatabase(databaseName: "MyInMemoryDatabase").UseInternalServiceProvider(sp);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            }

            if (Configuration.GetValue<bool>("IS_MIGRATING"))
            {
                Console.WriteLine("WARNING: IS_MIGRATING is true. This should not happen in production.");
                return;
            }
            #endregion

            #region Match Database
            var MATCH_DB_MYSQL_CONNECTION_STRING = GetOptionalEnvironmentVariable<string>(Configuration, "MATCH_DB_MYSQL_CONNECTION_STRING", null);
            // if a connectionString is set use mysql, else use InMemory
            if (MATCH_DB_MYSQL_CONNECTION_STRING != null)
            {
                // Add context as Transient instead of Scoped, as Scoped lead to DI error and does not have advantages under non-http conditions
                services.AddDbContext<MatchContext>(o => { o.UseMySql(MATCH_DB_MYSQL_CONNECTION_STRING); }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            }
            else
            {
                Console.WriteLine("WARNING: Using InMemoryDatabase!");

                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<MatchContext>((sp, options) =>
                    {
                        options.UseInMemoryDatabase(databaseName: "MyInMemoryDatabase").UseInternalServiceProvider(sp);
                    }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            }
            #endregion

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                OpenApiInfo interface_info = new OpenApiInfo { Title = "Mentor Interface", Version = "v1", };
                options.SwaggerDoc("v1", interface_info);

                // Generate documentation based on the Docstrings provided.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
            });
            #endregion

            #region Rabbit
            // Read environment variables
            var AMQP_URI = GetRequiredEnvironmentVariable<string>(Configuration, "AMQP_URI");
            #endregion

            #region Other worker services
            services.AddTransient<IMessageProcessor, MessageProcessor>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "[SERVICE NAME]");
            });
            #endregion

            #region Prometheus
            app.UseMetricServer(METRICS_PORT);
            #endregion

            #region Run Migrations
            // migrate if this is not an inmemory database
            if (services.GetRequiredService<MatchContext>().Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            {
                services.GetRequiredService<MatchContext>().Database.Migrate();
            }
            #endregion

        }

        /// <summary>
        /// Attempt to retrieve an Environment Variable
        /// Throws ArgumentNullException is not found.
        /// </summary>
        /// <typeparam name="T">Type to retreive</typeparam>
        private static T GetRequiredEnvironmentVariable<T>(IConfiguration config, string key)
        {
            T value = config.GetValue<T>(key);
            if (value == null)
            {
                throw new ArgumentNullException(
                    $"{key} is missing, Configure the `{key}` environment variable.");
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Attempt to retrieve an Environment Variable
        /// Returns default value if not found.
        /// </summary>
        /// <typeparam name="T">Type to retreive</typeparam>
        private static T GetOptionalEnvironmentVariable<T>(IConfiguration config, string key, T defaultValue)
        {
            var stringValue = config.GetSection(key).Value;
            try
            {
                T value = (T)Convert.ChangeType(stringValue, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                return value;
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine($"Env var [ {key} ] not specified. Defaulting to [ {defaultValue} ]");
                return defaultValue;
            }
        }
    }
}
