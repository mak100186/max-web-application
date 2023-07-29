using FluentAssertions;

using Kindred.Infrastructure.Hosting.WebApi.Extensions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Responses;
using Kindred.Rewards.Plugin.Reward.Mappings;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;
using Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Mappings;

[TestFixture]
[Category("Unit")]
public class DomainToApiMappingTests : MappingTestBase<RewardDomainToApiMapping>
{
    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    [TestCase(RewardType.Freebet)]
    public void MapFromDomainToEntitlementApiShouldSucceed(RewardType rewardType)
    {
        // Arrange
        var src = RewardClaimBuilder.CreateBonusClaim(rewardType, x
            => x.Terms
                .WithMinimumOdds(10)
                .WithMaxExtraWinnings(100));

        // Act
        var mapped = Mapper.Map<RewardEntitlementApiModel>(src);

        // Assert
        mapped.Should().NotBeNull("mapped");
        mapped.CustomerId.Should().BeEquivalentTo(src.CustomerId);

        mapped.Rn.Should().Be(new RewardRn(Guid.Parse(src.RewardId)).ToString());

        RewardParametersShouldMapToRewardTerms(
            mapped.RewardParameters,
            src.Terms);

        RewardParametersShouldMapToMultiConfig(mapped.DomainRestriction.MultiConfig, src.Terms.RewardParameters, rewardType);

        OddLimitsShouldMapCorrectly(mapped.DomainRestriction.OddLimits, src.Terms.Restrictions.OddLimits);

        mapped.DomainRestriction.FilterContestRefs
            .Should()
            .BeNullOrEmpty();

        mapped.DomainRestriction.FilterContestTypes
            .Should()
            .BeNullOrEmpty();

        mapped.DomainRestriction.FilterContestCategories
            .Should()
            .BeNullOrEmpty();

        mapped.DomainRestriction.FilterOutcomes
            .Should()
            .BeNullOrEmpty();
    }

    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    [TestCase(RewardType.Freebet)]
    public void MapFromDomainToRewardApiModelShouldSucceed(RewardType rewardType)
    {
        // Arrange
        var src = RewardBuilder.CreateReward<RewardDomainModel>(rewardType, x =>
            x.Terms
                .WithMinimumOdds(10)
                .WithMaxExtraWinnings(100));

        // Act
        var mapped = Mapper.Map<RewardApiModel>(src);

        // Assert
        mapped.Should().NotBeNull("mapped");
        mapped.CustomerId.Should().BeEquivalentTo(src.CustomerId);

        mapped.RewardRn.Should().Be(new RewardRn(Guid.Parse(src.RewardId)).ToString());

        RewardParametersShouldMapToRewardTerms(
            mapped.RewardParameters,
            src.Terms);

        RewardParametersShouldMapToMultiConfig(mapped.DomainRestriction.MultiConfig, src.Terms.RewardParameters, rewardType);

        OddLimitsShouldMapCorrectly(mapped.DomainRestriction.OddLimits, src.Terms.Restrictions.OddLimits);

        mapped.DomainRestriction.FilterContestRefs
            .Should()
            .BeNullOrEmpty();

        mapped.DomainRestriction.FilterContestTypes
            .Should()
            .BeNullOrEmpty();

        mapped.DomainRestriction.FilterContestCategories
           .Should()
           .BeNullOrEmpty();

        mapped.DomainRestriction.FilterOutcomes
            .Should()
            .BeNullOrEmpty();
    }

    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    [TestCase(RewardType.Freebet)]
    public void MapFromRewardClaimDomainModelToRewardEntitlementApiModel(RewardType rewardType)
    {
        // Arrange
        var src = RewardClaimBuilder.CreateBonusClaim(rewardType,
            x => x.Terms.WithMinimumOdds(10).WithMaxExtraWinnings(100));

        // Act
        var mapped = Mapper.Map<RewardEntitlementApiModel>(src);

        // Assert
        mapped.CustomerId.Should().Be(src.CustomerId);

        mapped.Rn.Should().Be(new RewardRn(Guid.Parse(src.RewardId)).ToString());

        mapped.DateTimeRestrictions.RemainingClaimsPerInterval.Should().Be(src.ClaimLimit);
        mapped.DateTimeRestrictions.ClaimsPerInterval.Should().Be(src.Terms.Restrictions.ClaimsPerInterval);
        mapped.DateTimeRestrictions.ExpiryDateTime.Should().Be(src.Terms.Restrictions.ExpiryDateTime);
        mapped.DateTimeRestrictions.StartDateTime.Should().Be(src.Terms.Restrictions.StartDateTime);
        mapped.DateTimeRestrictions.ClaimInterval.Should().Be(src.Terms.Restrictions.ClaimInterval);
        mapped.DateTimeRestrictions.ClaimAllowedPeriod.Should().Be(src.Terms.Restrictions.ClaimAllowedPeriod);
        mapped.DateTimeRestrictions.TimezoneId.Should().Be(src.Terms.Restrictions.TimezoneId);

        RewardParametersShouldMapToRewardTerms(
            mapped.RewardParameters,
            src.Terms);

        RewardParametersShouldMapToMultiConfig(mapped.DomainRestriction.MultiConfig, src.Terms.RewardParameters, rewardType);

        OddLimitsShouldMapCorrectly(mapped.DomainRestriction.OddLimits, src.Terms.Restrictions.OddLimits);
    }

    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    [TestCase(RewardType.Freebet)]
    public void MapFromRewardClaimDomainModelToClaimEntitlementResponse(RewardType rewardType)
    {
        // Arrange
        var src = RewardClaimBuilder.CreateBonusClaim(rewardType,
            x => x.Terms.WithMinimumOdds(10).WithMaxExtraWinnings(100));

        // Act
        var mapped = Mapper.Map<ClaimEntitlementResponse>(src);

        // Assert
        mapped.Reward.CustomerId.Should().Be(src.CustomerId);
        mapped.Reward.Rn.Should().Be(new RewardRn(Guid.Parse(src.RewardId)).ToString());
        mapped.Status.Should().Be(src.Status.ToString());
        mapped.Bet.Rn.Should().Be(src.Bet.Rn);

        RewardParametersShouldMapToRewardTerms(mapped.Reward.RewardParameters, src.Terms);
    }

    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    [TestCase(RewardType.Freebet)]
    public void MapFromRewardClaimDomainModelToClaimItemApiModel(RewardType rewardType)
    {
        // Arrange
        var src = RewardClaimBuilder.CreateBonusClaim(rewardType,
            x => x.Terms.WithMinimumOdds(10).WithMaxExtraWinnings(100));

        // Act
        var mapped = Mapper.Map<ClaimItemApiModel>(src);

        // Assert
        mapped.CustomerId.Should().Be(src.CustomerId);
        mapped.RewardRn.Should().Be(new RewardRn(Guid.Parse(src.RewardId)).ToString());
        mapped.Status.Should().Be(src.Status.ToString());
        mapped.BetRef.Should().Be(src.Bet.Rn);

        RewardParametersShouldMapToRewardTerms(mapped.RewardParameters, src.Terms);
    }

    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    [TestCase(RewardType.Freebet)]
    public void RewardClaimDomainModelToGetClaimsResponse(RewardType rewardType)
    {
        // Arrange
        var list = LoopUtilities.CreateMany(() => RewardClaimBuilder.CreateBonusClaim(rewardType,
            x => x.Terms.WithMinimumOdds(10).WithMaxExtraWinnings(100)));
        var src = new PagedResponse<RewardClaimDomainModel>
        {
            Items = list
        };

        var mapped = Mapper.Map<GetClaimsResponse>(src);
        var claimItemApiModels = mapped.Items.ToList();
        claimItemApiModels.Should().NotBeNull();
        for (var i = 0; i < src.Items.Count; i++)
        {
            claimItemApiModels[i].CurrencyCode.Should().Be(src.Items[i].CurrencyCode);
            claimItemApiModels[i].RewardRn.Should().Be(new RewardRn(Guid.Parse(src.Items[i].RewardId)).ToString());
            claimItemApiModels[i].RewardId.Should().Be(Guid.Parse(src.Items[i].RewardId).ToString());
            claimItemApiModels[i].InstanceId.Should().Be(src.Items[i].InstanceId);
            claimItemApiModels[i].CustomerId.Should().Be(src.Items[i].CustomerId);
            claimItemApiModels[i].Status.Should().Be(src.Items[i].Status.ToString());
            claimItemApiModels[i].RewardType.Should().Be(src.Items[i].Type.ToString());
            claimItemApiModels[i].RewardName.Should().Be(src.Items[i].PromotionName);
            claimItemApiModels[i].CouponRef.Should().Be(src.Items[i].CouponRn);
            claimItemApiModels[i].BetRef.Should().Be(src.Items[i].BetRn);
            claimItemApiModels[i].IntervalId.Should().Be(src.Items[i].IntervalId);
            claimItemApiModels[i].UsageId.Should().Be(src.Items[i].UsageId);
            claimItemApiModels[i].UpdatedOn.Should().Be(src.Items[i].UpdatedOn);
            claimItemApiModels[i].RewardPayoutAmount.Should().Be(src.Items[i].RewardPayoutAmount);
        }
    }

    private static void RewardParametersShouldMapToMultiConfig(MultiConfigApiModel multiConfig, IDictionary<string, string> rewardParameters,
        RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Profitboost:
            case RewardType.Freebet:
                multiConfig.MinStages.ToString()
                    .Should()
                    .BeEquivalentTo(rewardParameters[RewardParameterKey.MinStages]);

                multiConfig.MaxStages.ToString()
                    .Should()
                    .BeEquivalentTo(rewardParameters[RewardParameterKey.MaxStages]);

                multiConfig.MinCombinations.ToString()
                    .Should()
                    .BeEquivalentTo(rewardParameters[RewardParameterKey.MinCombinations]);

                multiConfig.MaxCombinations.ToString()
                    .Should()
                    .BeEquivalentTo(rewardParameters[RewardParameterKey.MaxCombinations]);

                multiConfig.FilterFormulae
                    .Should()
                    .BeEquivalentTo(rewardParameters[RewardParameterKey.AllowedFormulae].Split(","));
                break;
            case RewardType.Uniboost:
            case RewardType.UniboostReload:
                break;
            default:
                return;
        }
    }

    private static void RewardParametersShouldMapToRewardTerms(RewardParameterApiModelBase rewardParameterApiModel,
        RewardTerms rewardTermsExpectation)
    {
        RewardParametersBaseShouldMapCorrectly(rewardParameterApiModel, rewardTermsExpectation);

        switch (rewardParameterApiModel)
        {
            case ProfitBoostParametersApiModel profitboostParameters:
                profitboostParameters.MaxStakeAmount
                    .ToString()
                    .Should()
                    .BeEquivalentTo(rewardTermsExpectation.RewardParameters[RewardParameterKey.MaxStakeAmount]);
                JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(rewardTermsExpectation.RewardParameters[RewardParameterKey.LegTable])
                    .Select(x =>
                        profitboostParameters.LegTable[int.Parse(x.Key)]
                            .Should()
                            .Be(decimal.Parse(x.Value)));
                break;
            case FreeBetParametersApiModel freeBetParameters:
                freeBetParameters.Amount
                    .ToString()
                    .Should()
                    .BeEquivalentTo(rewardTermsExpectation.RewardParameters[RewardParameterKey.Amount]);

                break;
            case UniBoostReloadParametersApiModel uniboostReloadParameters:
                uniboostReloadParameters.MaxStakeAmount
                    .ToString()
                    .Should()
                    .BeEquivalentTo(rewardTermsExpectation.RewardParameters[RewardParameterKey.MaxStakeAmount]);
                uniboostReloadParameters.OddsLadderOffset
                    .ToString()
                    .Should()
                    .BeEquivalentTo(rewardTermsExpectation.RewardParameters[RewardParameterKey.OddsLadderOffset]);
                uniboostReloadParameters.Reload.MaxReload.Should()
                    .Be(rewardTermsExpectation.Restrictions.Reload.MaxReload);
                uniboostReloadParameters.Reload.StopOnMinimumWinBets.Should()
                    .Be(rewardTermsExpectation.Restrictions.Reload.StopOnMinimumWinBets);
                break;
            case UniBoostParametersApiModel uniboostParameters:
                uniboostParameters.MaxStakeAmount
                    .ToString()
                    .Should()
                    .BeEquivalentTo(rewardTermsExpectation.RewardParameters[RewardParameterKey.MaxStakeAmount]);
                uniboostParameters.OddsLadderOffset
                    .ToString()
                    .Should()
                    .BeEquivalentTo(rewardTermsExpectation.RewardParameters[RewardParameterKey.OddsLadderOffset]);
                break;
        }
    }

    private static void OddLimitsShouldMapCorrectly(OddLimitsApiModel oddLimits, OddLimitsConfig expected)
    {
        oddLimits.MinimumStageOdds
            .Should()
            .Be(expected.MinimumStageOdds);
        oddLimits.MinimumCompoundOdds
            .Should()
            .Be(expected.MinimumCompoundOdds);
    }

    private static void RewardParametersBaseShouldMapCorrectly(RewardParameterApiModelBase mapped,
        RewardTerms expectation)
    {
        mapped.MaxExtraWinnings
            .ToString()
            .Should()
            .BeEquivalentTo(expectation.RewardParameters[RewardParameterKey.MaxExtraWinnings]);
    }
}
