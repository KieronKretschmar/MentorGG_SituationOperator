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
using RabbitCommunicationLib.Queues;
using RabbitCommunicationLib.Interfaces;
using RabbitCommunicationLib.TransferModels;
using RabbitCommunicationLib.Producer;
using ZoneReader;
using EquipmentLib;
using SituationOperator.Communications;
using SituationOperator.SituationManagers;
using SituationDatabase.Models;
using StackExchange.Redis;
using Moq;
using SituationOperator.Helpers.SubscriptionConfig;
using System.Net.Http;

namespace SituationOperator
{
    public class Startup
    {
        private bool IsDevelopment => Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == Environments.Development;
        
        private const ushort AMQP_PREFETCH_COUNT_DEFAULT = 0;

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
            services.AddControllers()
                .AddNewtonsoftJson(x =>
                {
                    x.UseMemberCasing();
                    // Serialize longs (steamIds) as strings
                    x.SerializerSettings.Converters.Add(new LongToStringConverter());
                    x.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });

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

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                OpenApiInfo interface_info = new OpenApiInfo { Title = "SituationOperator", Version = "v1", };
                options.SwaggerDoc("v1", interface_info);

                // Generate documentation based on the Docstrings provided.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
            });
            #endregion

            #region HTTP Clients

            // Add HTTP clients for communication with other services in the cluster
            services.AddConnectedHttpService(ConnectedServices.MatchRetriever, Configuration, "MATCHRETRIEVER_URL_OVERRIDE");
            #endregion

            #region Rabbit
            if (IsDevelopment && GetOptionalEnvironmentVariable<bool>(Configuration, "MOCK_RABBIT", false))
            {
                Console.WriteLine("Using mocked rabbit classes.");
                // Use mocked producer
                services.AddTransient<IProducer<SituationExtractionReport>>(services =>
                {
                    var mockProducer = new Mock<IProducer<SituationExtractionReport>>().Object;
                    return mockProducer;
                });
            }
            else
            {
                // Read environment variables
                var AMQP_URI = GetRequiredEnvironmentVariable<string>(Configuration, "AMQP_URI");

                // Consumer for instructions from Fanout / DemoCentral
                var AMQP_PREFETCH_COUNT = GetOptionalEnvironmentVariable<ushort>(Configuration, "AMQP_PREFETCH_COUNT", AMQP_PREFETCH_COUNT_DEFAULT);
                var AMQP_EXTRACTION_INSTRUCTIONS = GetRequiredEnvironmentVariable<string>(Configuration, "AMQP_EXTRACTION_INSTRUCTIONS");
                var extractionQueue = new QueueConnection(AMQP_URI, AMQP_EXTRACTION_INSTRUCTIONS);
                services.AddHostedService<RabbitConsumer>(serviceProvider =>
                {
                    return new RabbitConsumer(
                        serviceProvider.GetRequiredService<ILogger<RabbitConsumer>>(),
                        serviceProvider,
                        extractionQueue,
                        AMQP_PREFETCH_COUNT);
                });

                // Producer for Reports to DemoCentral
                var AMQP_EXTRACTION_REPLY = GetRequiredEnvironmentVariable<string>(Configuration, "AMQP_EXTRACTION_REPLY");
                var callbackQueue = new QueueConnection(AMQP_URI, AMQP_EXTRACTION_REPLY);
                services.AddTransient<IProducer<SituationExtractionReport>>(sp =>
                {
                    return new Producer<SituationExtractionReport>(callbackQueue);
                });
            }
            #endregion

            #region MatchDataSet retrieval
            if (IsDevelopment && GetOptionalEnvironmentVariable<bool>(Configuration, "MOCK_MATCHDATASET_PROVIDER", false))
            {
                services.AddTransient<IMatchDataSetProvider, MockMatchDataSetProvider>();
            }
            else
            {
                // If redis is to be skipped, add IConnectionMultiplexer as null
                if (GetOptionalEnvironmentVariable<bool>(Configuration, "SKIP_REDIS", false))
                {
                    services.AddTransient<IMatchDataSetProvider, MatchDataSetProvider>(sp =>
                    {
                        return new MatchDataSetProvider(
                            sp.GetRequiredService<ILogger<MatchDataSetProvider>>(),
                            null,
                            sp.GetRequiredService<IHttpClientFactory>());
                    });

                }
                else
                {
                    // Add ConnectionMultiplexer as singleton as it is made to be reused
                    // see https://stackexchange.github.io/StackExchange.Redis/Basics.html
                    var REDIS_CONFIGURATION_STRING = GetRequiredEnvironmentVariable<string>(Configuration, "REDIS_CONFIGURATION_STRING");
                    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(REDIS_CONFIGURATION_STRING));
                    services.AddTransient<IMatchDataSetProvider, MatchDataSetProvider>();
                }

            }
            #endregion

            #region ZoneReader
            var ZONEREADER_RESOURCE_PATH = GetRequiredEnvironmentVariable<string>(Configuration, "ZONEREADER_RESOURCE_PATH");
            services.AddSingleton<IZoneReader, FileReader>(services =>
            {
                return new FileReader(services.GetService<ILogger<FileReader>>(), ZONEREADER_RESOURCE_PATH);
            });
            #endregion

            #region EquipmentLib
            // Wrap EquipmentProvider inside EquipmentHelper to simplify usage.
            var EQUIPMENT_CSV_DIRECTORY = GetRequiredEnvironmentVariable<string>(Configuration, "EQUIPMENT_CSV_DIRECTORY");
            var EQUIPMENT_ENDPOINT = GetOptionalEnvironmentVariable<string>(Configuration, "EQUIPMENT_ENDPOINT", null);
            services.AddSingleton<IEquipmentHelper, EquipmentHelper>(x =>
            {
                var equipmentProvider = new EquipmentProvider(
                    x.GetService<ILogger<EquipmentProvider>>(),
                    EQUIPMENT_CSV_DIRECTORY,
                    EQUIPMENT_ENDPOINT);

                return new EquipmentHelper(equipmentProvider);
            });
            #endregion

            #region Subscription Configuration

            if (!GetOptionalEnvironmentVariable<bool>(Configuration, "MOCK_SUBSCRIPTION_LOADER", false))
            {
                services.AddSingleton<ISubscriptionConfigProvider, SubscriptionConfigLoader>();
            }
            else
            {
                Console.WriteLine(
                    "WARNING: SubscriptionConfigLoader is mocked and will return mocked values!");
                services.AddSingleton<ISubscriptionConfigProvider, MockedSubscriptionConfigLoader>();
            }

            #endregion

            #region SituationManagers

            #region Misplays - Singleplayer
            services.AddTransient<ISituationManager, SmokeFailManager>();
            services.AddTransient<ISituationManager, DeathInducedBombDropManager>();
            services.AddTransient<ISituationManager, SelfFlashManager>();
            services.AddTransient<ISituationManager, TeamFlashManager>();
            services.AddTransient<ISituationManager, RifleFiredWhileMovingManager>();
            services.AddTransient<ISituationManager, UnnecessaryReloadManager>();
            services.AddTransient<ISituationManager, PushBeforeSmokeDetonatedManager>();
            #endregion

            #region Highlights - Singleplayer
            services.AddTransient<ISituationManager, EffectiveHeGrenadeManager>();
            services.AddTransient<ISituationManager, KillWithOwnFlashAssistManager>();
            services.AddTransient<ISituationManager, ClutchManager>();
            services.AddTransient<ISituationManager, HighImpactRoundManager>();
            #endregion

            #endregion

            #region Other worker services
            services.AddTransient<IMessageProcessor, MessageProcessor>();
            services.AddTransient<IMatchWorker, MatchWorker>();
            services.AddTransient<ISituationManagerProvider, SituationManagerProvider>();
            services.AddTransient<IBurstHelper, BurstHelper>();
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
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Situation Operator");
            });
            #endregion

            #region Prometheus
            app.UseMetricServer(METRICS_PORT);
            #endregion

            #region Run Migrations
            // migrate if this is not an inmemory database
            if (services.GetRequiredService<SituationContext>().Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory" && IsDevelopment == false)
            {
                services.GetRequiredService<SituationContext>().Database.Migrate();
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
