using AutoMapper;

using Kindred.Rewards.Rewards.UnitTests.Common;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Mappings;

public abstract class MappingTestBase<TMapping> : TestBase where TMapping : Profile, new()
{
    [Test]
    public void ValidateMappingConfiguration()
    {
        Mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
