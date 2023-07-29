using AutoFixture;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

namespace Kindred.Rewards.Rewards.UnitTests.Common.AutoFixtureCustomizations;
internal class CreateRewardRequestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<RewardRequest>(composer
            => composer
                .With(p => p.RewardType, ObjectBuilder.GetRandomEnum<RewardType>().ToString()));
    }
}
