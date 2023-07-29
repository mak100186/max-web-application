using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.Rn;

using Microsoft.Extensions.Logging;

namespace Kindred.Rewards.Core.Mapping.Converters;

public class RewardRnConverter : IConverter<string, RewardRn>
{
    private readonly ILogger<RewardRnConverter> _logger;

    public RewardRnConverter(ILogger<RewardRnConverter> logger)
    {
        _logger = logger;
    }

    public RewardRn Convert(string sourceObject)
    {
        if (string.IsNullOrWhiteSpace(sourceObject))
        {
            _logger.LogWarning("Null RewardRn string representation received");

            return default;
        }

        using var scope = _logger.BeginScope(new Dictionary<string, object> { { nameof(RewardRn), sourceObject } });
        if (Guid.TryParse(sourceObject, out var id))
        {
            return new(id);
        }

        try
        {
            return new(sourceObject);
        }
        catch (Exception)
        {
            _logger.LogInformation("Failed to convert RewardRn string representation to object");

            return default;
        }
    }

    public (IEnumerable<RewardRn> converted, IEnumerable<string> invalid) Convert(IEnumerable<string> sourceObjects)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object> { { nameof(RewardRn), sourceObjects } });
        List<RewardRn> convertedRns = new();
        List<string> invalidRns = new();

        foreach (var rewardRn in sourceObjects)
        {
            var converted = Convert(rewardRn);
            if (converted != null)
            {
                convertedRns.Add(converted);
                continue;
            }

            invalidRns.Add(rewardRn);
        }

        _logger.LogInformation("Converted the list of RewardRn string representations to objects. These could not be parsed: [{invalidRns}]", invalidRns.ToCsv(","));

        return (convertedRns, invalidRns);
    }
}
