using Kindred.Rewards.Core.Authorization.Middleware;

using Microsoft.AspNetCore.Http;

namespace Kindred.Rewards.Core.Authorization;

public class AuthorisationService : IAuthorisationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorisationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetUsername()
    {
        var usernameExists = _httpContextAccessor.HttpContext
            .Items
            .TryGetValue(AuthConstants.AuthorisationUsername, out var username);

        return usernameExists ? username?.ToString() : null;
    }
}


