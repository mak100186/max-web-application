namespace Kindred.Rewards.Core.WebApi.Requests;

public class GetRewardsByFilterRequest
{
    public long CustomerId { get; set; }

    public DateTimeOffset? UpdatedDateFromUtc { get; set; }

    public DateTimeOffset? UpdatedDateToUtc { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
