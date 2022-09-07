using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using CodingChallenge.Application;
using CodingChallenge.Application.TVMaze.Commands.Burn;
using CodingChallenge.Application.TVMaze.Commands.Scrape;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace CodingChallenge.EventQueueProcessor;

public class EventQueueLambdaClass
{
    public ILogger logger;
    public IConfiguration configuration;
    public IServiceProvider serviceProvider;
    public AWSAppProject awsApplication;

    public const int TryCountLimit = 20;

    public EventQueueLambdaClass()
    {
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables().Build();
        awsApplication = new AWSAppProject();
        configuration.GetSection(Constants.APPLICATION_ENVIRONMENT_VAR_PREFIX).Bind(awsApplication);

        logger = SetupLogger();

        var services = new ServiceCollection();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationBaseDependencies();
        services.AddInfrastructureDependencies(configuration, logger);
        services.AddSingleton(logger);
        services.AddSingleton(awsApplication);

        services.AddTransient<TVMazeScrapeCommandController, TVMazeScrapeCommandController>();
        services.AddTransient<TVMazeLambdaRunner, TVMazeLambdaRunner>();
    }


    public ILogger SetupLogger()
    {
        return new CustomLambdaLoggerProvider(new CustomLambdaLoggerConfig()
        {
            LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            InfrastructureProject = awsApplication

        }).CreateLogger(nameof(EventQueueLambdaClass));
    }

    public async Task HandleAsync(SQSEvent sQSEvent, ILambdaContext context)
    {
        logger.LogInformation($"Handling SQS Event");
        if (sQSEvent == null || sQSEvent.Records == null || !sQSEvent.Records.Any())
        {
            logger.LogInformation($"No records are found");
            return;
        }
        var runner = serviceProvider.GetService<TVMazeLambdaRunner>();

        foreach (var record in sQSEvent.Records)
        {
            try
            {
                logger.LogInformation($"log debug {record.Body}");

                var taskObject = JsonConvert.DeserializeObject<AddScrapeTaskCommand>(record.Body);
                int startIndex = Convert.ToInt32(taskObject!.StartIndex);
                int endIndex = Convert.ToInt32(taskObject.EndIndex);

                var results = new List<ScrapeCommandResponse>();
                var tasks = new List<Task>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    logger.LogInformation($"{i} - sending scrape command for index {i}");
                    tasks.Add(runner!.SendScrapeCommand(i));
                }

                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    var result = ((Task<ScrapeCommandResponse>)task).Result;
                    results.Add(result);

                    logger.LogInformation($"{result.Index} - asyncresponse received");

                    if (result.IsSuccess && result.CastListEmpty == true)
                    {
                        logger.LogInformation($"{result.Index} - success call && cast list is empty. We won't try again");
                        continue;
                    }

                    if (!result.IsSuccess && result.NotFound == true)
                    {
                        logger.LogInformation($"{result.Index} - Item Not found. We won't try again");
                        continue;
                    }

                    if (!result.IsSuccess)
                    {
                        logger.LogInformation($"{result.Index} -- NOT SUCCESS. Will retry if already not in db or try count not exceeded.");

                        if (taskObject.TryCount < TryCountLimit)
                        {
                            var indexResult = await runner!.GetItemByIdAsync(new Application.TVMaze.Queries.GetTVMazeItemByIndexQuery(result.Index.ToString()));
                            if (indexResult == null)
                            {
                                logger.LogInformation($"{result.Index} -- Item is not in database");

                                var newOrder = new AddScrapeTaskCommand(result.Index, result.Index, taskObject.TryCount + 1);
                                logger.LogInformation($"{result.Index} - Try Count -> {taskObject.TryCount} - Adding a new task for a failed task. Id -> {result.Index}.");

                                await runner.AddScrapeTaskAsync(newOrder);
                            }
                            else
                            {
                                logger.LogInformation($"{result.Index} -- Item already exists in database");
                            }
                        } else {
                            logger.LogInformation($"{result.Index} -- Try Count limit exceeded.");
                        }
                    }
                    else
                    {
                        logger.LogInformation($"{result.Index} -- Saved in DB!");
                    }
                }
            }

            catch (Exception ex)
            {
                logger.LogInformation($"error processing queue... Message: {ex.Message}. StackTrace {ex.StackTrace}. exception type -> {ex.GetType()}");
                logger.LogInformation($"inner exception error processing queue... Message: {ex.InnerException?.Message}. StackTrace {ex.InnerException?.StackTrace}");
                throw;
            }
        }
    }

}
