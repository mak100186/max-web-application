using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Maxx.Plugin.Shared.Abstractions;

public interface IHealthProbe
{
    void AddError(string source, string message);

    HealthCheckResult GetStatus();
}
