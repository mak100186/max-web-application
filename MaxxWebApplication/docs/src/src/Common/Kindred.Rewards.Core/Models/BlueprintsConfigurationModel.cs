namespace Kindred.Rewards.Core.Models;

public class BlueprintsConfigurationModel
{
    public string DefaultTemplate { get; set; }
    public List<string> LockedTemplates { get; set; }
}
public class Template
{
    public string Title { get; set; }
    public string Comments { get; set; }
}
