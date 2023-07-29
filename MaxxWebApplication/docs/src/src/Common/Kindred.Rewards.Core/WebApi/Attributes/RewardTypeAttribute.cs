namespace Kindred.Rewards.Core.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RewardTypeAttribute : Attribute
{
    public RewardTypeAttribute(string rewardType)
    {
        RewardType = rewardType;
    }

    public string RewardType { get; }
}
