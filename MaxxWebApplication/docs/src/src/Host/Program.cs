using Kindred.Infrastructure.Core.Configuration.SelfReferencing;
using Kindred.Infrastructure.Hosting.WebApi.Configuration;
using Kindred.Infrastructure.Hosting.WebApi.Health;
using Kindred.Rewards.Plugin.Host;
using Kindred.Rewards.Plugin.Host.Health;
using Kindred.Rewards.Plugin.Host.Logging;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Serilog;

Log.Logger = LoggingSetup.CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureAppConfiguration(c => { c.AddReferencingProvider(); });

    builder.Host.AddLogger();

    builder.Services.ConfigureServices(builder.Configuration);

    var app = builder.Build();

    app.AddRequestLogging();

    //await app.ApplyMigrations();

    //app.Services.AddKafkaHandlers();

    //app.Services.InitializeOddsLadder();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger()
            .UseSwaggerUI()
            .UseDeveloperExceptionPage();
    }

    app.UseSwaggerK8s()
        .UseForwardedHeaders()
        .UseRouting()
        //.UseMiddleware<JwtMiddleware>()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/api/health",
                new HealthCheckOptions
                {
                    ResponseWriter = HealthResponseFormatter.WriteResponse
                });
        });

    //app.CreateSchemas(KspMessageHelper.GetKspMessagesTypes(Assembly.GetAssembly(typeof(RewardStatus))));

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
