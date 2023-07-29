using System.Collections.Concurrent;
using System.Text;

using Maxx.Plugin.Shared.Abstractions;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Maxx.Plugin.Shared.Health;

public class HealthProbe : IHealthProbe
{
    private readonly ConcurrentDictionary<string, string> _errors = new();

    public void AddError(string source, string message)
    {
        if (_errors.ContainsKey(source))
        {
            _errors.TryRemove(source, out var error);

            message = $"{error} | {message}";
        }

        _errors.TryAdd(source, message);
    }

    public HealthCheckResult GetStatus()
    {
        return _errors.Any()
            ? HealthCheckResult.Unhealthy(FormatErrors(), data: _errors.ToDictionary(pair => pair.Key, pair => (object)pair.Value))
            : HealthCheckResult.Healthy();
    }

    private string FormatErrors()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("The following errors occurred:");

        foreach (var source in _errors.Keys)
        {
            if (_errors.TryGetValue(source, out var error))
            {
                stringBuilder.AppendLine($"{source} : {error}");
            }
        }

        return stringBuilder.ToString();
    }
}
