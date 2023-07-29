namespace Kindred.Rewards.Core.WebApi.Requests;

public class GetPromotionTemplatesRequest
{
    public bool IncludeDisabled { get; set; }

    public string TemplateKey { get; set; }
}
