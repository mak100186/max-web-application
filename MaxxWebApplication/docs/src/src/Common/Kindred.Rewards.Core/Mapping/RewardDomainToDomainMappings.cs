using AutoMapper;

using Kindred.Rewards.Core.Extensions;
using Kindred.Rewards.Core.Models;
using Kindred.Rewards.Core.Models.RewardClaims;

namespace Kindred.Rewards.Core.Mapping;

public sealed class RewardDomainToDomainMappings : Profile
{
    public RewardDomainToDomainMappings()
    {
        //todo: do we need this
        CreateMap<RewardDomainModel, RewardDomainModel>();

        CreateMap<RewardDomainModel, RewardClaimDomainModel>(MemberList.None)
            .ForMember(d => d.Reward, opt => opt.MapFrom(s => s))
            .ForMember(d => d.InstanceId, opt => opt.MapFrom(s => Guid.NewGuid().ToString()))
            .ForMember(d => d.CustomerId, opt => opt.Ignore())
            .ForMember(d => d.PromotionName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.CouponRn, opt => opt.Ignore())
            .ForMember(d => d.BetRn, opt => opt.Ignore())
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
            .ForMember(d => d.Hash, opt => opt.MapFrom(src => src.GetSha256()));

        CreateMap<BatchClaimItemDomainModel, RewardClaimDomainModel>(MemberList.None)
            .ForMember(d => d.RewardId, opt => opt.MapFrom(s => s.RewardId))
            .ForMember(d => d.BetRn, opt => opt.MapFrom(s => s.Bet.Rn))
            .ForMember(d => d.Bet, opt => opt.MapFrom(s => s.Bet))
            .ForMember(d => d.Hash, opt => opt.MapFrom(s => s.Hash));

        CreateMap<RewardClaimUsageDomainModel, RewardClaimDomainModel>(MemberList.None)
            .ForMember(d => d.IntervalId, opt => opt.MapFrom(s => s.IntervalId))
            .ForMember(d => d.ClaimLimit, opt => opt.MapFrom(s => s.ClaimRemaining))
            .ForMember(d => d.UsageId, opt => opt.MapFrom(s => s.CurrentUsageId));

        CreateMap<RewardClaimDomainModel, RewardDomainModel>(MemberList.None)
            .ForMember(d => d.RewardId, o => o.MapFrom(s => s.RewardId))
            .ForMember(d => d.Category, o => o.MapFrom(s => s.Category))
            .ForMember(d => d.Purpose, o => o.MapFrom(s => s.Purpose))
            .ForMember(d => d.SubPurpose, o => o.MapFrom(s => s.SubPurpose))
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type))
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments))
            .ForMember(d => d.RewardRules, o => o.MapFrom(s => s.RewardRules))
            .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.CreatedBy))
            .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn))
            .ForMember(d => d.UpdatedBy, o => o.MapFrom(s => s.UpdatedBy))
            .ForMember(d => d.UpdatedOn, o => o.MapFrom(s => s.UpdatedOn))
            .ForMember(d => d.Tags, o => o.MapFrom(s => s.Tags))
            .ForMember(d => d.CountryCode, o => o.MapFrom(s => s.CountryCode))
            .ForMember(d => d.CurrencyCode, o => o.MapFrom(s => s.CurrencyCode))
            .ForMember(d => d.Jurisdiction, o => o.MapFrom(s => s.Jurisdiction))
            .ForMember(d => d.State, o => o.MapFrom(s => s.State))
            .ForMember(d => d.Brand, o => o.MapFrom(s => s.Brand))
            .ForMember(d => d.Terms, o => o.MapFrom(s => s.Terms));
    }
}
