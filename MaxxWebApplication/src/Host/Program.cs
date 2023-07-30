
using MaxxWebApplication.Dependency;
using MaxxWebApplication.Health;
using MaxxWebApplication.Logging;

using Serilog;

Log.Logger = LoggingSetup.CreateBootstrapLogger();

try
{
    var app = WebApplication.CreateBuilder(args)
        .ConfigureServices()
        .Build();

    app
        .ConfigureDevelopmentEnvironment()
        .AddRequestLogging()
        .UseForwardedHeaders()
        .UseHttpsRedirection()
        .UseRouting()
        .UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/api/health", new() { ResponseWriter = HealthResponseFormatter.WriteResponse });
        })
        .ConfigureApp();

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

