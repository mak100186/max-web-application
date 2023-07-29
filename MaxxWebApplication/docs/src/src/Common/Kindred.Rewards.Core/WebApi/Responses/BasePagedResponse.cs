namespace Kindred.Rewards.Core.WebApi.Responses;

public class BasePagedResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int RowCount { get; set; }
}
