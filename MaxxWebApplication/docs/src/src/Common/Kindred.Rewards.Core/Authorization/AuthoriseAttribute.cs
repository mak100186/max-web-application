using Microsoft.AspNetCore.Mvc.Filters;

namespace Kindred.Rewards.Core.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthoriseAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // TODO: Implement once we want to add authentication
        //var user = context.HttpContext.Items[AuthConstants.AuthorisationUsername];

        //if (user == null)
        //    context.Result = new JsonResult(new { message = AuthConstants.Unauthorised })
        //    {
        //        StatusCode = StatusCodes.Status404NotFound
        //    };
    }
}
