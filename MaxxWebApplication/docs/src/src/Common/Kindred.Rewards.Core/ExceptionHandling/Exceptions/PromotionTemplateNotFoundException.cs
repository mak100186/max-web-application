namespace Kindred.Rewards.Core.ExceptionHandling.Exceptions;

public class PromotionTemplateNotFoundException : Exception
{
    public PromotionTemplateNotFoundException(string code)
        : base($"Cannot find promotion template with template key {code}")
    {
    }
}
