using AutoMapper;

using Kindred.Rewards.Core.Mapping.Converters;
using Kindred.Rewards.Core.Models.Rn;

namespace Kindred.Rewards.Core.Mapping;
public class RewardRnIdResolver : IMemberValueResolver<object, object, string, string>
{
    private readonly IConverter<string, RewardRn> _rewardRnConverter;

    public RewardRnIdResolver(IConverter<string, RewardRn> rewardRnConverter)
    {
        _rewardRnConverter = rewardRnConverter;
    }

    public string Resolve(object source, object destination, string sourceMember, string destMember,
        ResolutionContext context)
    {
        return _rewardRnConverter.Convert(sourceMember)?.GuidValue;
    }
}
