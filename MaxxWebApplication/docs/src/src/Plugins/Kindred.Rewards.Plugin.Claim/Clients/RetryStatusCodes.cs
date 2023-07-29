using System.Net;

namespace Kindred.Rewards.Plugin.Claim.Clients;

public static class RetryStatusCodes
{
    public static Dictionary<HttpStatusCode, bool> StatusCodesWorthRetrying = new()
    {
        { HttpStatusCode.RequestTimeout, true },
        { HttpStatusCode.TooManyRequests, true },
        { HttpStatusCode.InternalServerError, true },
        { HttpStatusCode.BadGateway, true },
        { HttpStatusCode.ServiceUnavailable, true },
        { HttpStatusCode.GatewayTimeout, true },
    };
}
