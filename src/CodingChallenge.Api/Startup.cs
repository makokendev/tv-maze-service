using CodingChallenge.Api.Controllers;
using CodingChallenge.Application;
using CodingChallenge.Infrastructure;
using CodingChallenge.Infrastructure.Logging;

namespace CodingChallenge.Api;

public class Startup
{
    public ILogger logger;
    public IConfiguration configuration;
    public IServiceProvider serviceProvider;
    public AWSAppProject awsApplication;

    public Startup()
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

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationBaseDependencies();
        services.AddInfrastructureDependencies(configuration, logger);
        services.AddSingleton(logger);
        services.AddSingleton(awsApplication);
    }

    public ILogger SetupLogger()
    {
        return new CustomLambdaLoggerProvider(new CustomLambdaLoggerConfig()
        {
            LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            InfrastructureProject = awsApplication

        }).CreateLogger(nameof(ShowController));
    }

}