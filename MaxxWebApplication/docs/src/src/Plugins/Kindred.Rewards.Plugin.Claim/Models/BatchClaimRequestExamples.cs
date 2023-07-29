using Kindred.Rewards.Core.WebApi.Payloads.BetModel;
using Kindred.Rewards.Core.WebApi.Requests;
using Kindred.Rewards.Plugin.Claim.Extensions;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Claim.Models;

public class BatchClaimRequestExamples : IMultipleExamplesProvider<BatchClaimRequest>
{
    public IEnumerable<SwaggerExample<BatchClaimRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            nameof(BatchClaimRequest),
            new BatchClaimRequest
            {
                CustomerId = "12345",
                CouponRn = Guid.NewGuid().ToString(),
                CurrencyCode = "AUD",
                Claims = new()
                {
                    new()
                    {
                        Rn = Guid.NewGuid().ToString(),
                        Hash = Guid.NewGuid().ToString().GetSha256(),
                        Bet = new()
                        {
                            RequestedStake = 3.0m,
                            Formula = "none",
                            Rn = Guid.NewGuid().ToString(),
                            Stages = new List<CompoundStageApiModel>
                            {
                                new ()
                                {
                                    Market = "ksp:market.1:[football:201711202200:watford_vs_west_ham]:1x2",
                                    RequestedSelection = new() { Outcome = "watford" },
                                    Odds = new() { RequestedPrice = 1.5m }
                                }
                            },
                            Status = "pending",
                            RequestedCombinations = new List<CombinationApiModel>
                            {
                                new ()
                                {
                                    Rn = Guid.NewGuid().ToString(),
                                    Selections = new List<SelectionApiModel> { new () { Outcome = "watford" } }
                                }
                            }
                        }
                    }
                }
            });
    }
}
