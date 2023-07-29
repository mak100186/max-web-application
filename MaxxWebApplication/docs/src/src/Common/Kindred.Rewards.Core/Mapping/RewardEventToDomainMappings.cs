using AutoMapper;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models.Events;

using MinotaurBet = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Bet;
using MinotaurCombination = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Combination;
using MinotaurCombinationSettlement = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.CombinationSettlement;
using MinotaurCompoundStage = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.CompoundStage;
using MinotaurSegment = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Segment;
using MinotaurSelection = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Selection;
using MinotaurUnitSelection = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.UnitSelection;

namespace Kindred.Rewards.Core.Mapping;

public class RewardEventToDomainMappings : Profile
{
    public RewardEventToDomainMappings()
    {
        CreateMap<MinotaurBet, RewardBet>(MemberList.None)
            .ForMember(d => d.BetRn, opt => opt.MapFrom(s => s.Rn))
            .ForMember(d => d.Stake, opt => opt.MapFrom(s => Convert.ToDecimal(s.Stake)))
            .ForMember(d => d.Formula, opt => opt.MapFrom(s => s.Formula.ToString()))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToRewardDomainEnum()))
            .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId))
            .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(s => s.Currency.ToString()))
            .ForMember(d => d.BetOutcome, opt => opt.MapFrom(s => s.AcceptedCombinations.GetBetOutcome()))
            .ForMember(d => d.AcceptedCombinations, opt => opt.MapFrom(s => s.AcceptedCombinations))
            .ForMember(d => d.Stages, opt => opt.MapFrom(s => s.Stages));

        CreateMap<MinotaurCombination, Combination>()
            .ForMember(d => d.Rn, opt => opt.MapFrom(s => s.Rn))
            .ForMember(d => d.Settlement, opt => opt.MapFrom(s => s.Settlement))
            .ForMember(d => d.Selections, opt => opt.MapFrom(s => s.Selections));

        CreateMap<MinotaurCombinationSettlement, CombinationSettlement>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.Payoff, opt => opt.MapFrom(s => s.Payoff))
            .ForMember(d => d.Segments, opt => opt.MapFrom(s => s.Segments));

        CreateMap<MinotaurSegment, Segment>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status))
            .ForMember(d => d.Dividend, opt => opt.MapFrom(s => s.Dividend));

        CreateMap<MinotaurUnitSelection, Selection>()
            .ForMember(d => d.Outcome, opt => opt.MapFrom(s => s.Outcome));

        CreateMap<MinotaurCompoundStage, CompoundStage>()
            .ForMember(d => d.Market, opt => opt.MapFrom(s => s.Market))
            .ForMember(d => d.ContestType, opt => opt.MapFrom(s => s.Market.GetContestTypeFromMarket()))
            .ForMember(d => d.OriginalOdds, opt => opt.MapFrom(s => s.Odds.Price))
            .ForMember(d => d.Selection, opt => opt.MapFrom(s => s.Selection));

        CreateMap<MinotaurSelection, Selection>()
            .ForMember(d => d.Outcome, opt => opt.MapFrom(s => s.Outcome));

    }
}
