using Kindred.Rewards.Plugin.Base.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kindred.Rewards.Plugin.Host.Health;

public class RewardsPluginServiceCheck : IHealthCheck
{
    private readonly ILogger<RewardsPluginServiceCheck> _logger;
    private readonly IHealthProbe _healthProbe;

    public RewardsPluginServiceCheck(ILogger<RewardsPluginServiceCheck> logger, IHealthProbe healthProbe)
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
            _logger.LogError("Kindred.Rewards.Plugins health check failed with {@Exception}", ex);
            return HealthCheckResult.Unhealthy("Kindred.Rewards.Plugins is Unhealthy", ex);
        }
    }
}
