using System.Text.RegularExpressions;

using AutoMapper;

using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;
using Kindred.Infrastructure.Core.Extensions.Extensions;
using Kindred.Offer.PriceManager.Event.Models.MessageModels;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardConfiguration;

using Newtonsoft.Json;

using DomainBetStatus = Kindred.Rewards.Core.Enums.BetStatus;
using Odds = Kindred.Rewards.Core.Models.Odds;
using WalletEnums = Kindred.Customer.WalletGateway.ExternalModels.Common.Enums;
using WalletMinotaurEnums = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums;

namespace Kindred.Rewards.Core.Extensions;

public static class Extensions
{

    public static OddsLadder ToOddsLadder(this OddsLadderMessageModel oddsLadderMessageModel)
    {
        return new()
        {
            ContestType = oddsLadderMessageModel.ContestType.ToString(),
            InPlayOddsLadder = oddsLadderMessageModel.InPlayOddsLadder.Select(x => x.ToOdds()).ToList(),
            PreGameOddsLadder = oddsLadderMessageModel.PreGameOddsLadder.Select(x => x.ToOdds()).ToList(),
        };
    }
    public static Odds ToOdds(this OddsMessageModel oddsMessageModel)
    {
        return new()
        {
            Display = oddsMessageModel.Display,
            Key = oddsMessageModel.Key
        };
    }

    private static string? GetContestTypeFromMarket(this string market)
    {
        var match = Regex.Match(market, "^ksp:market.*\\[(?<contestType>\\w+):.*$", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups["contestType"].Value : null;
    }

    public static DomainBetStatus? ToRewardDomainEnum(this WalletEnums.BetStatus betStatus)
    {
        return betStatus switch
        {
            WalletEnums.BetStatus.Settled => DomainBetStatus.Settled,
            WalletEnums.BetStatus.Cancelled => DomainBetStatus.Cancelled,
            _ => null
        };
    }

    // TODO Remove when rewardbet is removed
    public static BetOutcome? GetBetOutcome(this IEnumerable<Combination> combinations)
    {
        if (combinations.Any(x => x.Settlement.Status is not WalletMinotaurEnums.SettlementCombinationStatus.Resolved))
        {
            return null;
        }

        if (combinations.All(x => x.Settlement.Segments.All(y => y.Status is WalletMinotaurEnums.SettlementSegmentStatus.Won)))
        {
            return BetOutcome.Winning;
        }

        if (combinations.All(x => x.Settlement.Segments.All(y => y.Status is WalletMinotaurEnums.SettlementSegmentStatus.Refunded)))
        {
            return BetOutcome.FullRefund;
        }

        if (combinations.All(x => x.Settlement.Segments.Any(y => y.Status is WalletMinotaurEnums.SettlementSegmentStatus.PartWon or WalletMinotaurEnums.SettlementSegmentStatus.PartRefunded)))
        {
            return BetOutcome.WinningAndPartialRefund;
        }

        return BetOutcome.Losing;
    }
    public static IMappingExpression<TSrc, TDest> MapEditableBase<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> m) where TDest : BaseEditableDataModel<int>
    {
        return m
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.UpdatedOn, o => o.Ignore());

    }

    public static IMappingExpression<TSrc, TDest> MapFromEditableBase<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> m) where TSrc : BaseEditableDataModel<int>
    {
        return m.ForSourceMember(d => d.Id, o => o.DoNotValidate()).ForSourceMember(d => d.CreatedOn, o => o.DoNotValidate());
    }

    public static IMappingExpression<TSrc, TDest> MapToRewardBase<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> m)
        where TSrc : Core.Infrastructure.Data.DataModels.Reward
        where TDest : RewardDomainModel
    {
        return m
            .ForSourceMember(d => d.CreatedOn, o => o.DoNotValidate())
            .ForSourceMember(d => d.UpdatedOn, o => o.DoNotValidate())

            .ForMember(d => d.CancellationReason, o => o.MapFrom(s => s.CancellationReason))
            .ForMember(d => d.IsCancelled, o => o.MapFrom(s => s.IsCancelled))
            .ForMember(d => d.RewardId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.Category, o => o.MapFrom(s => Enum.Parse<RewardType>(s.RewardType).ToRewardCategory()))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.RewardType))
            .ForMember(d => d.SourceInstanceId, o => o.MapFrom(s => s.SourceInstanceId))
            .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.CountryCode))
            .ForMember(d => d.Jurisdiction, o => o.MapFrom(s => s.Jurisdiction))
            .ForMember(d => d.State, o => o.MapFrom(s => s.State))
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Purpose, o => o.MapFrom(s => s.Purpose))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.SubPurpose, o => o.MapFrom(s => s.SubPurpose))
            .ForMember(d => d.CustomerFacingName, o => o.MapFrom(s => s.CustomerFacingName))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.RewardRules, o => o.MapFrom(s => s.RewardRules))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.AsUtcDateTime()))
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn.AsUtcDateTime()))
            .ForMember(d => d.Terms, o => o.MapFrom((s, d) => JsonConvert.DeserializeObject<RewardTerms>(s.TermsJson)))
            .AfterMap(
                (src, dest) =>
                    {
                        dest.Terms.Restrictions.StartDateTime = DateTime.SpecifyKind(dest.Terms.Restrictions.StartDateTime, DateTimeKind.Utc);
                        dest.Terms.Restrictions.ExpiryDateTime = DateTime.SpecifyKind(dest.Terms.Restrictions.ExpiryDateTime, DateTimeKind.Utc);
                        if (!string.IsNullOrWhiteSpace(src.ContestStatus))
                        {
                            dest.Terms.Restrictions.AllowedContestStatuses = Enum.Parse<ContestStatus>(src.ContestStatus);
                        }
                    });
    }
    public static IMappingExpression<TSrc, TDest> MapRewardBase<TSrc, TDest>(
        this IMappingExpression<TSrc, TDest> m)
        where TSrc : RewardDomainModel
        where TDest : Core.Infrastructure.Data.DataModels.Reward
    {
        return m
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.UpdatedOn, o => o.Ignore())

            .ForMember(d => d.StartDateTime, o => o.MapFrom((s, d) => s.Terms.Restrictions.StartDateTime.ApplyUtc()))
            .ForMember(d => d.ExpiryDateTime, o => o.MapFrom((s, d) => s.Terms.Restrictions.ExpiryDateTime.ApplyUtc()))

            .ForMember(d => d.ContestStatus, o => o.MapFrom((s, d) => s.Terms.Restrictions.AllowedContestStatuses))

            .ForMember(d => d.IsCancelled, o => o.MapFrom(s => s.IsCancelled))
            .ForMember(d => d.CancellationReason, o => o.MapFrom(s => s.CancellationReason))
            .ForMember(d => d.SourceInstanceId, o => o.MapFrom(s => s.SourceInstanceId))
            .ForMember(d => d.Id, o => o.MapFrom(s => s.RewardId))
            .ForMember(d => d.RewardType, o => o.MapFrom(s => s.Type))
            .ForMember(d => d.TermsJson, o => o.MapFrom((s, d) => JsonConvert.SerializeObject(s.Terms)))
            .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.CountryCode))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.Jurisdiction, o => o.MapFrom(s => s.Jurisdiction))
            .ForMember(d => d.State, o => o.MapFrom(s => s.State))
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Purpose, o => o.MapFrom(s => s.Purpose))
            .ForMember(d => d.SubPurpose, o => o.MapFrom(s => s.SubPurpose))
            .ForMember(d => d.CustomerFacingName, o => o.MapFrom(s => s.CustomerFacingName))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy));
    }
}
