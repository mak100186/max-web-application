using Maxx.Plugin.Common.Extensions;

using Microsoft.Extensions.Configuration;

namespace Maxx.TestCommon.EndpointMock;

public static class Extensions
{
    public const string TestDataSection = "testData";

    public static Mocks<TRequest, TResponse>? GetMocksForMethod<TRequest, TResponse>(this IConfiguration config, string client, string method)
    {
        var section = config.GetSection(TestDataSection);
        var serializedList = section.Get<List<Mocks<TRequest, TResponse>>>();

        return serializedList?.FirstOrDefault(x => x.Client == client && x.Method == method);
    }
    public static Mock<TRequest, TResponse>? GetApplicableMock<TRequest, TResponse>(this Mocks<TRequest, TResponse> mockTestData, string parameterName, TRequest paramToCompare)
    {
        var applicableMocks = new List<Mock<TRequest, TResponse>>();

        foreach (var mock in mockTestData.MockData)
        {
            if (mock.Request.ContainsKey(parameterName))
            {
                var requestParameter = mock.Request[parameterName];

                if (paramToCompare.IsEqual(requestParameter))
                {
                    applicableMocks.Add(mock);
                }
            }
        }

        if (applicableMocks.Count > 1)
        {
            throw new("Multiple applicable mocks founds");
            //todo: this could be a valid case where we want multiple mocks and we can randomize the responses. May be for a later date
        }

        return applicableMocks.SingleOrDefault();
    }
}
