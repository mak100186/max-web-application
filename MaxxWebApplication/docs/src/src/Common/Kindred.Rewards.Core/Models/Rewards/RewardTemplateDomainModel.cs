namespace Kindred.Rewards.Core.Models.Rewards;

public class RewardTemplateDomainModel
{
    public int Id { get; set; }
    public string TemplateKey { get; set; }
    public string Comments { get; set; }
    public string Title { get; set; }
    public bool Enabled { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
    public IList<RewardDomainModel> Rewards { get; set; }
}
