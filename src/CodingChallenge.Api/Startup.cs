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

        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

    }

    public ILogger SetupLogger()
    {
        return new CustomLambdaLoggerProvider(new CustomLambdaLoggerConfig()
        {
            LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
            InfrastructureProject = awsApplication

        }).CreateLogger(nameof(ShowController));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        //if (env.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI();
        //}

        app.UseHttpsRedirection();

        //app.UseAuthorization();

        // app.MapControllers();
        app.UseRouting();
        app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        //app.MapControllers();

        //app.Run();
        // if (env.IsDevelopment())
        // {
        //     app.UseDeveloperExceptionPage();
        // }

        // //app.UseHttpsRedirection();
        // app.UseRouting();
        // app.UseCors(MyAllowSpecificOrigins);
        // app.UseAuthorization();
        // app.UseEndpoints(endpoints =>
        // {
        //     endpoints.MapControllers();
        // });

        // app.UseSwagger();
        // app.UseSwaggerUI(c =>
        // {
        //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        //     c.RoutePrefix = string.Empty;
        // });
    }

}