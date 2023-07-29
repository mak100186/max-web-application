﻿using Kindred.Rewards.Core.Enums;

namespace Kindred.Rewards.Core.Models.Events;

public class CombinationSettlement
{
    public IReadOnlyCollection<Segment> Segments { get; set; }
    public SettlementCombinationStatus Status { get; set; }
    public double Payoff { get; set; }
}
