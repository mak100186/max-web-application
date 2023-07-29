namespace Kindred.Rewards.Core.Exceptions;

public class RewardRnInvalidException : Exception
{
    public RewardRnInvalidException()
        : base("Could not parse the provided RewardRn. Supported values are the Rn identifier or the full Rn.")
    {
    }

    public RewardRnInvalidException(string rewardRn)
        : base($"Could not parse the provided RewardRns. Supported values are the Rn identifier or the full Rn. Provided Rn: {rewardRn}")
    {
    }

    public RewardRnInvalidException(IEnumerable<string> rewardRns)
        : base($"Could not parse the provided RewardRns. Supported values are the Rn identifier or the full Rn. Provided Rns: {string.Join(",", rewardRns.Select(x => x))}")
    {
    }
}
