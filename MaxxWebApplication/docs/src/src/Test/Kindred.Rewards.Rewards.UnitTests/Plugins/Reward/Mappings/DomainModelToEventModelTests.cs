
using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Mapping;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.Messages;
using Kindred.Rewards.Core.Models.Messages.RewardTemplate;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Mappings;

[TestFixture]
[Category("Unit")]
public class DomainModelToEventModelTests : MappingTestBase<RewardDomainToEventMapping>
{
    [Test]
    public void MapFromDomainModelToEventModelShouldSucceed()
    {
        var src = RewardClaimBuilder.CreateBonusClaim();
        var freeBetModel = src.Terms;

        var mapped = Mapper.Map<RewardClaim>(src);

        mapped.Type.Should().Be(src.Type.ToString());
        mapped.CustomerId.Should().BeEquivalentTo(src.CustomerId);
        mapped.InstanceId.Should().BeEquivalentTo(src.InstanceId);
        mapped.CouponRef.Should().BeEquivalentTo(src.CouponRn);
        mapped.BetRef.Should().Be(src.BetRn);
        mapped.RewardPayoutAmount.Should().Be(src.RewardPayoutAmount);
        mapped.Terms.ShouldNotBeNull();
        mapped.Terms.SettlementTerms.ShouldNotBeNull();

        mapped.Terms.SettlementTerms.ReturnStake.Should().Be(freeBetModel.SettlementTerms.ReturnStake);

        mapped.Terms.RewardParameters.Keys.Should().BeEquivalentTo(freeBetModel.RewardParameters.Keys);
        mapped.Terms.RewardParameters.Values.Should().BeEquivalentTo(freeBetModel.RewardParameters.Values);
    }

    [Test]
    public void MapFormDomainModelToCancelEventShouldSucceed()
    {
        var src = RewardClaimBuilder.CreateBonusClaim();

        var mapped = Mapper.Map<RewardClaim>(src);
        mapped.CustomerId.Should().BeEquivalentTo(src.CustomerId);
        mapped.InstanceId.Should().BeEquivalentTo(src.InstanceId);
        mapped.Status.Should().BeEquivalentTo(src.Status.ToString());
    }

    [Test]
    public void MapFromDomainModelToRewardTemplateUpdatedEventShouldSucceed()
    {
        var reward = RewardBuilder.CreateReward<RewardDomainModel>(RewardType.Profitboost, null);

        var src = new RewardTemplateDomainModel
        {
            Rewards = new List<RewardDomainModel> { reward },
            TemplateKey = "test",
            CreatedBy = "user3245",
            UpdatedBy = "user1234"
        };

        var mapped = Mapper.Map<RewardTemplateUpdated>(src);

        mapped.Rewards
            .Select(x => x.RewardRn)
            .Should()
            .BeEquivalentTo(src.Rewards.Select(x => new RewardRn(Guid.Parse(x.RewardId)).ToString()));

        mapped.TemplateKey.Should().BeEquivalentTo(src.TemplateKey);

        mapped.CreatedBy.Should().BeEquivalentTo(src.CreatedBy);
        mapped.UpdatedBy.Should().BeEquivalentTo(src.UpdatedBy);
    }
}
