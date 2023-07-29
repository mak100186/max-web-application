using Kindred.Rewards.Core.Infrastructure.Data.DataModels;
using Kindred.Rewards.Core.Models;

namespace Kindred.Rewards.Core.Infrastructure.Data.Extensions;
internal static class TemplateMappings
{

    public static RewardTemplate ToRewardTemplate(this Template template)
    {
        return new()
        {
            Comments = template.Comments,
            Enabled = true,
            Key = template.Title,
            Title = template.Title,
            UpdatedOn = DateTime.UtcNow,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = string.Empty,
            UpdatedBy = string.Empty,
            RewardTemplateReward = new()
        };
    }

    public static ICollection<RewardTemplate> ToRewardTemplates(this IEnumerable<Template> templates)
    {
        return templates.Select(x => x.ToRewardTemplate()).ToList();
    }
}
