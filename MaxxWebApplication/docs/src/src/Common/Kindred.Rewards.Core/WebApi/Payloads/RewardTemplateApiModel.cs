namespace Kindred.Rewards.Core.WebApi.Payloads;

public class RewardTemplateApiModel
{
    public string TemplateKey { get; set; }
    public bool Enabled { get; set; }
    public string Comments { get; set; }
    public string Title { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
