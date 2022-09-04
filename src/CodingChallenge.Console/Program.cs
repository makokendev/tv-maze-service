﻿using CodingChallenge.Application;
using CodingChallenge.Infrastructure;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodingChallenge.Console;

partial class Program
{
    static IConfiguration? configuration;
    static ILogger? logger;
    static ServiceProvider? serviceProvider;
    public static AWSAppProject? awsApplication;

    static async Task Main(string[] args)
    {
        LoadConfiguration();
        SetupLogger();
        ConfigureServices(new ServiceCollection());
        var commandParser = serviceProvider!.GetService<TVMazeConsoleRunner>();
        var parser = Parser.Default.ParseArguments<CommandLineOptions>(args);
        await parser.WithParsedAsync(async options => await commandParser!.RunOptionsAsync(options));
        await parser.WithNotParsedAsync(async errs => await commandParser!.HandleParseErrorAsync(errs));
    }

    static void LoadConfiguration()
    {
        configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables().Build();
        awsApplication = new AWSAppProject();
        configuration.GetSection(Constants.APPLICATION_ENVIRONMENT_VAR_PREFIX).Bind(awsApplication);
    }

    static void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationBaseDependencies();
        services.AddInfrastructureDependencies(configuration!,logger!);
        services.AddSingleton(logger!);
        services.AddTransient<TVMazeScrapeCommandController, TVMazeScrapeCommandController>();
        services.AddTransient<TVMazeConsoleRunner, TVMazeConsoleRunner>();
        services.AddSingleton(awsApplication!);
        serviceProvider = services.BuildServiceProvider();
    }

    static void SetupLogger()
    {
        var loggerFactory = LoggerFactory.Create(
                builder => builder
                            // add debug output as logging target
                            //.AddDebug()
                            .AddConsole()
                            // set minimum level to log
                            .SetMinimumLevel(LogLevel.Information));
        logger = loggerFactory.CreateLogger("CodingChallenge Console App");
    }
}