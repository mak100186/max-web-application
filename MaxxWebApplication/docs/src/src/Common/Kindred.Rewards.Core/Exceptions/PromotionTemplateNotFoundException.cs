namespace Kindred.Rewards.Core.Exceptions;

public class PromotionTemplateNotFoundException : Exception
{
    public PromotionTemplateNotFoundException(string code)
        : base($"Cannot find promotion template with template key {code}")
    {
    }
}
