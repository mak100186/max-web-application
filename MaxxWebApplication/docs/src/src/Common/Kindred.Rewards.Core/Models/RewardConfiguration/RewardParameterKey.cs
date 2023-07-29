namespace Kindred.Rewards.Core.Models.RewardConfiguration;

public static class RewardParameterKey
{
    public const string Amount = "amount";
    public const string MaxStakeAmount = "maxStakeAmount";
    public const string MinimumStageOdds = "minimumStageOdds";
    public const string MinimumCompoundOdds = "minimumCompoundOdds";
    public const string MinStakeAmount = "minStakeAmount";
    public const string MaxExtraWinnings = "maxExtraWinnings";
    public const string LegTable = "legTable";

    //odds ladder
    public const string OddsLadderOffset = "oddsLadderOffset";
    public const string OddsLadderType = "oddsLadderType";

    //multi-config
    public const string AllowedFormulae = "allowedFormulae";
    public const string MinStages = "minStages";
    public const string MaxStages = "maxStages";
    public const string MinCombinations = "minCombinations";
    public const string MaxCombinations = "maxCombinations";

}
