using System.ComponentModel.DataAnnotations;

namespace Kindred.Rewards.Core.WebApi.Requests;

#nullable enable
public class PatchRewardRequest
{
    public string? Name { get; set; }

    [StringLength(300)]
    public string? Comments { get; set; }
}
