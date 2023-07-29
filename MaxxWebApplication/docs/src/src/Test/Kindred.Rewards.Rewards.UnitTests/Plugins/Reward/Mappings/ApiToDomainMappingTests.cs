using AutoFixture;

using FluentAssertions;

using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;
using Kindred.Rewards.Core.Models.RewardConfiguration;
using Kindred.Rewards.Core.Models.Rewards;
using Kindred.Rewards.Core.Models.Rn;
using Kindred.Rewards.Core.WebApi.Payloads;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Plugin.Reward.Mappings;
using Kindred.Rewards.Plugin.Reward.Models;
using Kindred.Rewards.Rewards.Tests.Common.DataBuilders;
using Kindred.Rewards.Rewards.Tests.Common.Extensions;
using Kindred.Rewards.Rewards.Tests.Common.Extensions.DataModifiers;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Kindred.Rewards.Rewards.UnitTests.Plugins.Reward.Mappings;

[TestFixture]
[Category("Unit")]
public class ApiToDomainMappingTests : MappingTestBase<RewardApiToDomainMapping>
{
    [TestCase(null, null, null)]
    [TestCase(null, "0 0 0 ? * * *", "UTC")]
    [TestCase("AUS Eastern Standard Time", "0 0 0 ? * * *", "AUS Eastern Standard Time")]
    [TestCase("AUS Eastern Standard Time", null, null)]
    public void ShouldDefaultTimezoneId(string timezoneId, string claimAllowedPeriod, string expected)
    {
        var src = RewardRequestBuilder.CreateRewardRequestForType<RewardRequest>(RewardType.Uniboost, reward =>
        {
            reward.Restrictions.ClaimAllowedPeriod = claimAllowedPeriod;
            reward.Restrictions.TimezoneId = timezoneId;
        });

        var mapped = Mapper.Map<RewardDomainModel>(src);

        mapped.Should().NotBeNull();
        mapped.Terms.Restrictions.TimezoneId.Should().Be(expected);
    }

    [Test]
    public void ShouldMapFromPatchRewardRequestToRewardPatchDomainModel()
    {
        // Arrange
        var request = new PatchRewardRequest { Comments = "hello comment", Name = "Joseph" };

        // Act
        var mapped = Mapper.Map<RewardPatchDomainModel>(request);

        // Assert
        mapped.ShouldNotBeNull();
        mapped.Comments.Should().Be("hello comment");
        mapped.Name.Should().Be("Joseph");
    }

    [Test]
    public void ShouldMapGetBonusesRequestToBonusFilterDomainModel()
    {
        // arrange
        var request = new GetRewardsRequest
        {
            CustomerId = "5150",
            UpdatedDateFromUtc = new DateTime(1984, 11, 5, 10, 04, 00, DateTimeKind.Utc),
            UpdatedDateToUtc = new DateTime(1984, 11, 12, 22, 04, 00, DateTimeKind.Utc),
            IncludeExpired = true,
            IncludeCancelled = false,
        };

        // act
        var mapped = Mapper.Map<RewardFilterDomainModel>(request);

        // assert
        mapped.CustomerId.Should().Be("5150");
        mapped.UpdatedDateFromUtc.Should().Be(request.UpdatedDateFromUtc?.UtcDateTime);
        mapped.UpdatedDateToUtc.Should().Be(request.UpdatedDateToUtc?.UtcDateTime);
        mapped.IncludeCancelled.Should().Be(request.IncludeCancelled);
        mapped.IncludeExpired.Should().Be(request.IncludeExpired);
    }

    [Test]
    public void ShouldMapGetClaimsByFilterRequestToCustomerRewardFilterDomainModel()
    {
        // arrange
        var request = new GetClaimsByFilterRequest
        {
            InstanceId = "InstanceId1",
            RewardRn = new RewardRn(Guid.NewGuid()).ToString(),
            RewardName = "NamingConvention1",
            CustomerId = "CustomerId1",
            CouponRef = "CouponRef1",
            BetRef = "2",
            RewardType = "RewardType1",
            ClaimStatus = "ClaimStatus1",
            BetOutcomeStatus = "BetOutcomeStatus1",
            UpdatedDateFromUtc = new DateTime(1984, 11, 5, 10, 04, 00, DateTimeKind.Utc),
            UpdatedDateToUtc = new DateTime(1984, 11, 12, 22, 04, 00, DateTimeKind.Utc)
        };

        // act
        var mapped = Mapper.Map<RewardClaimFilterDomainModel>(request);

        // assert
        mapped.InstanceId.Should().Be("InstanceId1");
        mapped.RewardRn.Should().Be(new RewardRn(request.RewardRn).GuidValue);
        mapped.RewardName.Should().Be("NamingConvention1");
        mapped.CouponRn.Should().Be("CouponRef1");
        mapped.BetRn.Should().Be("2");
        mapped.RewardType.Should().Be("RewardType1");
        mapped.ClaimStatus.Should().Be("ClaimStatus1");
        mapped.BetOutcomeStatus.Should().Be("BetOutcomeStatus1");
        mapped.UpdatedDateFromUtc.Should().Be(request.UpdatedDateFromUtc?.UtcDateTime);
        mapped.UpdatedDateToUtc.Should().Be(request.UpdatedDateToUtc?.UtcDateTime);
    }

    [Test]
    public void MapFromClaimApiModelToRewardClaimDomainModel()
    {
        // Arrange
        var bet = ObjectBuilder.CreateApiBet();
        var src = new ClaimApiModel
        { Bet = bet, Rn = new RewardRn(Guid.NewGuid()).ToString(), Hash = ObjectBuilder.CreateString() };

        // Act
        var mapped = Mapper.Map<BatchClaimItemDomainModel>(src);

        // Assert
        mapped.Bet.Rn.Should().Be(src.Bet.Rn);
        mapped.RewardId.Should().Be(new RewardRn(src.Rn).GuidValue);

        var domainMarketProducts = mapped.Bet.Stages.OrderBy(m => m.Market).ToList();
        var apiMarketProducts = src.Bet.Stages.OrderBy(mp => mp.Market).ToList();

        domainMarketProducts[0].Market.Should().Be(apiMarketProducts[0].Market);
        domainMarketProducts[0].RequestedOutcome.Should().Be(apiMarketProducts[0].RequestedSelection.Outcome);
    }

    [Test]
    public void MapFromBatchClaimRequestToBatchClaimDomainModel()
    {
        // Arrange
        var claims = Fixture.Build<ClaimApiModel>()
            .With(p => p.Rn, new RewardRn(Guid.NewGuid()).ToString())
            .CreateMany()
            .ToList();

        foreach (var stage in claims.SelectMany(claim => claim.Bet.Stages))
        {
            stage.Market = ObjectBuilder.CreateMarketKey();
        }

        var src = Fixture.Build<BatchClaimRequest>()
            .With(p => p.Claims, claims)
            .Create();

        // Act
        var mapped = Mapper.Map<BatchClaimDomainModel>(src);

        // Assert
        mapped.CouponRn.Should().Be(src.CouponRn);
        mapped.CustomerId.Should().Be(src.CustomerId);
        mapped.Claims.Select(c => c.RewardId).Should().BeEquivalentTo(src.Claims.Select(c => new RewardRn(c.Rn).GuidValue));
        mapped.Claims.Select(c => c.Bet.Rn).Should().BeEquivalentTo(src.Claims.Select(c => c.Bet.Rn));
        mapped.CurrencyCode.Should().Be(src.CurrencyCode);

        var domainMarketProducts = mapped.Claims.SelectMany(c => c.Bet.Stages).OrderBy(mp => mp.Market).ToList();
        var apiMarketProducts = src.Claims.SelectMany(c => c.Bet.Stages).OrderBy(mp => mp.Market).ToList();

        foreach (var expected in apiMarketProducts)
        {
            domainMarketProducts.Select(m => m.Market).Should().Contain(expected.Market);
        }
    }

    [Test]
    [TestCase(RewardType.Profitboost)]
    [TestCase(RewardType.Freebet)]
    public void MapFromProfitBoostOrFreeBetCreateRewardRequestToRewardDomainModel(RewardType reward)
    {
        // Arrange
        var request = RewardRequestBuilder.CreateRewardRequestForType<RewardRequest>(reward, x =>
            x.WithMinimumOdds(minimumStageOdds: 52));

        var validator = new RewardRequestValidator();

        // Act
        var mapped = Mapper.Map<RewardDomainModel>(request);

        var validationResult = validator.Validate(request);

        // Assert
        mapped.Terms.Restrictions.AllowedContestRefs.Should()
            .BeEquivalentTo(request.DomainRestriction.FilterContestRefs);

        mapped.Terms.Restrictions.AllowedContestTypes.Should()
            .BeEquivalentTo(request.DomainRestriction.FilterContestTypes);

        mapped.Terms.Restrictions.AllowedContestCategories.Should()
            .BeEquivalentTo(request.DomainRestriction.FilterContestCategories);

        mapped.Terms.Restrictions.AllowedOutcomes.Should()
            .BeEquivalentTo(request.DomainRestriction.FilterOutcomes);

        mapped.Terms.Restrictions.OddLimits.MinimumStageOdds.Should()
            .Be(request.DomainRestriction.OddLimits.MinimumStageOdds);

        RewardParametersShouldMapCorrectly(mapped, request.RewardParameters, request.DomainRestriction.MultiConfig);

        validationResult.Errors.Should().BeEmpty();

        // Arrange
        request = RewardRequestBuilder.CreateRewardRequestForType<RewardRequest>(reward, x =>
            x.WithMinimumOdds(minimumStageOdds: 52, minimumCompoundOdds: 52));

        // Act
        validationResult = validator.Validate(request);

        // Assert
        validationResult.Errors.Should().NotBeEmpty();
        validationResult.Errors.Count().Should().Be(2);
        validationResult.Errors[0].ErrorMessage.Should().StartWith("228|");
    }

    [Test]
    [TestCase(RewardType.UniboostReload)]
    [TestCase(RewardType.Uniboost)]
    public void MapFromUniboostOrUniboostReloadCreateRewardRequestToRewardDomainModel(RewardType reward)
    {
        // Arrange
        var request = RewardRequestBuilder.CreateRewardRequestForType<RewardRequest>(reward, x =>
            x.WithMinimumOdds(minimumStageOdds: 52));

        // Act
        var mapped = Mapper.Map<RewardDomainModel>(request);

        // Assert
        mapped.Terms.Restrictions.AllowedContestRefs.Should().BeNullOrEmpty();

        mapped.Terms.Restrictions.AllowedContestTypes.Should().BeNullOrEmpty();

        mapped.Terms.Restrictions.AllowedOutcomes.Should().BeNullOrEmpty();

        mapped.Terms.Restrictions.OddLimits.MinimumStageOdds.Should()
            .Be(request.DomainRestriction.OddLimits.MinimumStageOdds);

        RewardParametersShouldMapCorrectly(mapped, request.RewardParameters, request.DomainRestriction.MultiConfig);
    }

    private static void RewardParametersShouldMapCorrectly(RewardDomainModel mapped,
        RewardParameterApiModelBase expectation, MultiConfigApiModel multiConfigExpectation)
    {
        RewardParametersBaseShouldMapCorrectly(mapped, expectation);

        switch (expectation)
        {
            case ProfitBoostParametersApiModel profitboostParameters:
                mapped.Terms.RewardParameters[RewardParameterKey.MaxStakeAmount]
                    .Should()
                    .BeEquivalentTo(profitboostParameters.MaxStakeAmount.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MinStages].Should()
                    .BeEquivalentTo(multiConfigExpectation.MinStages.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MaxStages].Should()
                    .BeEquivalentTo(multiConfigExpectation.MaxStages.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MinCombinations].Should()
                    .BeEquivalentTo(multiConfigExpectation.MinCombinations.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MaxCombinations].Should()
                    .BeEquivalentTo(multiConfigExpectation.MaxCombinations.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.AllowedFormulae].Split(",").Should()
                    .BeEquivalentTo(multiConfigExpectation.FilterFormulae);

                JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(mapped.Terms.RewardParameters[RewardParameterKey.LegTable])
                    .Select(x => mapped.Terms.RewardParameters[x.Key]
                        .Should()
                        .BeEquivalentTo(profitboostParameters.LegTable[int.Parse(x.Key)].ToString()));
                break;
            case FreeBetParametersApiModel freeBetParameters:
                mapped.Terms.RewardParameters[RewardParameterKey.Amount]
                    .Should()
                    .BeEquivalentTo(freeBetParameters.Amount.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MinStages].Should()
                    .BeEquivalentTo(multiConfigExpectation.MinStages.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MaxStages].Should()
                    .BeEquivalentTo(multiConfigExpectation.MaxStages.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MinCombinations].Should()
                    .BeEquivalentTo(multiConfigExpectation.MinCombinations.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.MaxCombinations].Should()
                    .BeEquivalentTo(multiConfigExpectation.MaxCombinations.ToString());

                mapped.Terms.RewardParameters[RewardParameterKey.AllowedFormulae].Split(",").Should()
                    .BeEquivalentTo(multiConfigExpectation.FilterFormulae);

                break;
            case UniBoostReloadParametersApiModel uniboostReloadParameters:
                mapped.Terms.RewardParameters[RewardParameterKey.OddsLadderOffset]
                    .Should()
                    .BeEquivalentTo(uniboostReloadParameters.OddsLadderOffset.ToString());
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MinStages);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MaxStages);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MinCombinations);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MaxCombinations);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.AllowedFormulae);
                mapped.Terms.Restrictions.Reload.StopOnMinimumWinBets.Should()
                    .Be(uniboostReloadParameters.Reload.StopOnMinimumWinBets);
                mapped.Terms.Restrictions.Reload.MaxReload.Should().Be(uniboostReloadParameters.Reload.MaxReload);
                break;
            case UniBoostParametersApiModel uniboostParameters:
                mapped.Terms.RewardParameters[RewardParameterKey.MaxStakeAmount]
                    .Should()
                    .BeEquivalentTo(uniboostParameters.MaxStakeAmount.ToString());
                mapped.Terms.RewardParameters[RewardParameterKey.OddsLadderOffset]
                    .Should()
                    .BeEquivalentTo(uniboostParameters.OddsLadderOffset.ToString());
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MinStages);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MaxStages);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MinCombinations);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.MaxCombinations);
                mapped.Terms.RewardParameters.Should().NotContainKey(RewardParameterKey.AllowedFormulae);
                break;
        }
    }

    private static void RewardParametersBaseShouldMapCorrectly(RewardDomainModel mapped,
        RewardParameterApiModelBase expectation)
    {
        mapped.Terms.RewardParameters[RewardParameterKey.MaxExtraWinnings].Should()
            .BeEquivalentTo(expectation.MaxExtraWinnings.ToString());
    }
}
