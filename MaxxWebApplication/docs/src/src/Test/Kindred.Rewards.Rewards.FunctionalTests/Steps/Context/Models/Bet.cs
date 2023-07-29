namespace Kindred.Rewards.Rewards.FunctionalTests.Steps.Context.Models;


public class Bet
{
    public string Rn { get; set; }
    public decimal Stake { get; set; }
    public string Formula { get; set; }
    public List<Stage> Stages { get; set; } = new();
    public List<Combination> Combinations { get; set; } = new();
    public List<RewardClaim> RewardClaims { get; set; } = new();
    public string CustomerId { get; set; }
    public Settlement Settlement { get; set; } = new();
}

public class Combination
{
    public string Rn { get; set; }
    public List<Selection> Selection { get; set; } = new();
    public CombinationSettlement Settlement { get; set; } = new();
}

public class Stage
{
    public string Market { get; set; }
    public Odds Odds { get; set; }
    public Selection Selection { get; set; }
}

public class Odds
{
    public decimal Price { get; set; }
}

public class Selection
{
    public string Outcome { get; set; }
}

public class CombinationSettlement
{
    public string Status { get; set; } = "Pending";
    public List<CombinationSettlementSegment> Segments { get; set; } = new();
}

public class CombinationSettlementSegment
{
    public string Status { get; set; }
}

public class RewardClaim
{
    public string RewardRn { get; set; }
    public string ClaimRn { get; set; }

}

public class Settlement
{
    public decimal FinalPayoff { get; set; }
    public string Status { get; set; } = "Pending";
}
