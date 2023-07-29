using Kindred.Rewards.Core.WebApi.Requests;

using Swashbuckle.AspNetCore.Filters;

namespace Kindred.Rewards.Plugin.Template.Models;

public class CreateRewardTemplateRequestExamples : IMultipleExamplesProvider<CreateRewardTemplateRequest>
{
    public IEnumerable<SwaggerExample<CreateRewardTemplateRequest>> GetExamples()
    {
        yield return SwaggerExample.Create(
            nameof(CreateRewardTemplateRequest),
            new CreateRewardTemplateRequest
            {
                TemplateKey = "UB_VALUED_MEDIUM",
                Comments = ""
            });
    }
}
