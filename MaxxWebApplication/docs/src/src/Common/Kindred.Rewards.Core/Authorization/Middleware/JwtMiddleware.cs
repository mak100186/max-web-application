using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;

namespace Kindred.Rewards.Core.Authorization.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var handler = new JwtSecurityTokenHandler();

        var authorizationHeader = context.Request.Headers[AuthConstants.RequestHeader];

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            var token = handler.ReadJwtToken(authorizationHeader.ToString());

            context.Items[AuthConstants.AuthorisationUsername] = GetUsername(token.Payload);
        }
        await _next(context);
    }

    private string? GetUsername(JwtPayload payload)
    {
        if (payload.TryGetValue(AuthConstants.AuthorisationUsername, out var value))
        {
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                return value?.ToString();
            }
        }
        return string.Empty;
    }
}

