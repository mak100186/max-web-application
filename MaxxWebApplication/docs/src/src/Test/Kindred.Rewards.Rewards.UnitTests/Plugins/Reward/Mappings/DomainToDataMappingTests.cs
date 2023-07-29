using FluentAssertions;

using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Mappings;

[TestFixture]
[Category("Unit")]
public class DomainToDataMappingTests : MappingTestBase<RewardDomainToDataMapping>
{
    [Test]
    public void MapRewardClaimDomainModelToRewardClaim()
    {
        // Arrange
        var src = RewardClaimBuilder.CreateBonusClaim();

        // Act
        var mapped = Mapper.Map<RewardClaim>(src);

        // Assert
        mapped.CouponRn.Should().Be(src.CouponRn);
        mapped.BetRn.Should().Be(src.Bet.Rn);
        mapped.CustomerId.Should().Be(src.CustomerId);
        mapped.IntervalId.Should().Be(src.IntervalId);
        mapped.RewardId.Should().Be(src.RewardId);
        mapped.UsageId.Should().Be(src.UsageId);
        mapped.RewardPayoutAmount.Should().Be(src.RewardPayoutAmount);
        //mapped.NamingConvention.Should().Be(src.NamingConvention);
        mapped.CurrencyCode.Should().Be(src.CurrencyCode);
    }
}
