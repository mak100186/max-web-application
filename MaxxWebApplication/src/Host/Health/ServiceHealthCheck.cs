using Maxx.Plugin.Shared.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MaxxWebApplication.Health;

public class ServiceHealthCheck : IHealthCheck
{
    private readonly IHealthProbe _healthProbe;
    private readonly ILogger<ServiceHealthCheck> _logger;

    public ServiceHealthCheck(ILogger<ServiceHealthCheck> logger, IHealthProbe healthProbe)
    {
        _logger = logger;
        _healthProbe = healthProbe;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            return _healthProbe.GetStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError("MaxxWebApplication health check failed with {@Exception}", ex);

            return HealthCheckResult.Unhealthy("MaxxWebApplication is Unhealthy", ex);
        }
    }
}
