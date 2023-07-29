using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels;
using Kindred.Customer.WalletGateway.ExternalModels.ChimeraModels.Enums;
using Kindred.Customer.WalletGateway.ExternalModels.Common.Enums;
using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels;
using Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums;
using Kindred.Rewards.Core.Enums;
using Kindred.Rewards.Core.Extensions;

using BetStatus = Kindred.Customer.WalletGateway.ExternalModels.Common.Enums.BetStatus;
using Combination = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Combination;
using CompoundBet = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.CompoundBet;
using CompoundStage = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.CompoundStage;
using ExternalRewardClaim = Kindred.Customer.WalletGateway.ExternalModels.Common.RewardClaim;
using FixedOdds = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.FixedOdds;
using MinotaurCurrency = Kindred.Customer.WalletGateway.ExternalModels.Common.Enums.Currency;
using MinotaurSettlementCombinationStatus = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums.SettlementCombinationStatus;
using MinotaurSettlementSegmentStatus = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Enums.SettlementSegmentStatus;
using Selection = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.Selection;
using UnitSelection = Kindred.Customer.WalletGateway.ExternalModels.MinotaurModels.UnitSelection;

namespace Kindred.Rewards.Rewards.Tests.Common;

public static class MessageFactory
{
    public static Plugin.MessageConsumers.EventHandlers.CouponDeclined CreateCouponDeclinedMessage(IEnumerable<string> instanceIds)
    {
        var couponRef = Guid.NewGuid().ToString();

        var rewardClaims = instanceIds.Select(i => new ExternalRewardClaim { ClaimRn = i }).ToList();

        return new()
        {
            Actions = new[]
            {
                new CouponAction
                {
                    Coupon = couponRef
                }
            },
            Resources = new[]
            {
                new Coupon
                {
                    Bets = new List<Customer.WalletGateway.ExternalModels.ChimeraModels.Bet>
                    {
                        new()
                        {
                            RewardClaims = rewardClaims
                        }
                    },
                    Channel = Channel.Web,
                    Rn = couponRef,
                    Errors = Array.Empty<CouponError>(),
                    Status = CouponStatus.Declined,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            }
        };
    }

    public static Plugin.MessageConsumers.EventHandlers.CouponFailed CreateCouponFailedMessage(IEnumerable<string> instanceIds)
    {
        var couponRef = Guid.NewGuid().ToString();

        var rewardClaims = instanceIds.Select(i => new ExternalRewardClaim { ClaimRn = i }).ToList();

        return new()
        {
            Actions = new[]
            {
                new CouponAction
                {
                    Coupon = couponRef
                }
            },
            Resources = new[]
            {
                new Coupon
                {
                    Bets = new List<Customer.WalletGateway.ExternalModels.ChimeraModels.Bet>
                    {
                        new ()
                        {
                            RewardClaims = rewardClaims
                        }
                    },
                    Channel = Channel.Web,
                    Rn = couponRef,
                    Errors = Array.Empty<CouponError>(),
                    Status = CouponStatus.Failed,
                    CreatedAt = DateTimeOffset.UtcNow
                }
            }
        };
    }


    public static SettlementMessage CreateSettlementMessage<T>(
        SettlementActionType type,
        BetStatus betStatus = BetStatus.Settled,
        MinotaurSettlementCombinationStatus settlementCombinationStatus = MinotaurSettlementCombinationStatus.Resolved,
        MinotaurSettlementSegmentStatus settlementSegmentStatus = MinotaurSettlementSegmentStatus.Won,
        string instanceId = "instanceId",
        string outcomeId = "outcomeId",
        string customerId = "customerId",
        string betRn = "betRn",
        string combinationRn = "combinationRn",
        double price = 4.5f,
        double stake = 1.0f,
        string rewardName = "rewardName",
        string rewardRn = "rewardRn",
        string marketId = "marketId",
        MinotaurCurrency currency = MinotaurCurrency.AUD,
        RewardType rewardClaimType = RewardType.Freebet,
        RewardClaimStatus rewardClaimStatus = RewardClaimStatus.Claimed,
        ICollection<CompoundStage> stages = null) where T : SettlementAction, new()
    {
        if (stages == null)
        {
            stages = new List<CompoundStage>
            {
                new()
                {
                    Selection = new UnitSelection
                    {
                        Outcome = outcomeId
                    },
                    Market = marketId,
                    Odds = new FixedOdds
                    {
                        Price = price
                    }
                }
            };
        }

        var message = CreateSettlementMessageWithoutAction(betStatus, settlementCombinationStatus,
            settlementSegmentStatus, instanceId, outcomeId, customerId, betRn, combinationRn, price, stake, rewardName,
            rewardRn, currency, rewardClaimType, rewardClaimStatus, stages);

        var bet = message.Resources.First();
        var segments = bet.AcceptedCombinations.First().Settlement.Segments;
        var betSettlement = bet.Settlement;

        message.Actions = new List<SettlementAction>
        {
            new T
            {
                Combination = combinationRn,
                PreviousBetSettlement = betSettlement,
                PreviousCombinationSettlement = new()
                {
                    Payoff = price,
                    Status = settlementCombinationStatus,
                    Segments = segments,
                    Stages = stages
                }
            }
        };

        return message;
    }

    public static SettlementMessage CreateSettlementMessageWithoutAction(
        BetStatus betStatus,
        MinotaurSettlementCombinationStatus settlementCombinationStatus,
        MinotaurSettlementSegmentStatus settlementSegmentStatus,
        string instanceId,
        string outcomeId,
        string customerId,
        string betRn,
        string combinationRn,
        double price,
        double stake,
        string rewardName,
        string rewardRn,
        MinotaurCurrency currency,
        RewardType rewardClaimType,
        RewardClaimStatus rewardClaimStatus,
        ICollection<CompoundStage> stages = null)
    {


        var betSettlement = new BetSettlement
        {
            ComputedPayoff = price,
            FinalPayoff = price,
            Progress = new()
            {
                Unresolved = settlementCombinationStatus == MinotaurSettlementCombinationStatus.Unresolved ? 1 : 0,
                Resolved = settlementCombinationStatus == MinotaurSettlementCombinationStatus.Resolved ? 1 : 0,
                Pending = settlementCombinationStatus == MinotaurSettlementCombinationStatus.Pending ? 1 : 0,
                Combinations = 1
            },
            Status = settlementCombinationStatus
        };

        var segments = new List<Segment>
        {
            new()
            {
                Status = settlementSegmentStatus,
                Deductions = null,
                Dividend = price,
                ResultRevision = 1273,
                Split = null
            }
        };

        var message = new SettlementMessage
        {
            Resources = new List<Customer.WalletGateway.ExternalModels.MinotaurModels.Bet>
            {
                new CompoundBet
                {
                    AcceptedCombinations = new List<Combination>
                    {
                        new()
                        {
                            Rn = combinationRn,
                            Selections = new List<Selection>
                            {
                                new()
                                {
                                    Outcome = outcomeId
                                }
                            },
                            Settlement = new()
                            {
                                Status = settlementCombinationStatus,
                                Payoff = price,
                                Segments = segments
                            }
                        }
                    },
                    Settlement = betSettlement,
                    CustomerId = customerId,
                    Currency = currency,
                    Status = betStatus,
                    Rn = betRn,
                    Stake = stake,
                    Formula = Formula.singles,
                    Stages = stages,
                    RewardClaims = new List<ExternalRewardClaim>
                    {
                        new()
                        {
                            ClaimRn = instanceId,
                            RewardRn = rewardRn,
                            ClaimMetadata = new()
                            {
                                RewardName = rewardName,
                                Status = rewardClaimStatus.ToString(),
                                CurrencyCode = nameof(currency),
                                Reward = new()
                                {
                                    Type = rewardClaimType.ToString(),
                                    Category = rewardClaimType.ToRewardCategory().ToString()
                                }
                            }
                        }
                    }
                }
            }
        };

        return message;
    }
}
