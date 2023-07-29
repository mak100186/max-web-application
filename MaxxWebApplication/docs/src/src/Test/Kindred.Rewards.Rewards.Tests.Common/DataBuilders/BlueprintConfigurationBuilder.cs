using Kindred.Rewards.Core.Models;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class BlueprintConfigurationBuilder
{
    public static BlueprintsConfigurationModel CreateConfiguration()
    {
        return new()
        {
            DefaultTemplate = "DefaultTemplate",
            LockedTemplates = ObjectBuilder.CreateManyStrings()
        };
    }
}
