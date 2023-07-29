using System.Linq.Expressions;

using Microsoft.Extensions.Logging;

using Moq;

namespace Kindred.Rewards.Rewards.UnitTests.Common;

public static class MockExtensions
{
    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, int expectedCalls)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(expectedCalls));
    }

    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, Expression<Func<object, Type, bool>> match, int expectedCalls)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(match),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Exactly(expectedCalls));
    }

    public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, string message, int expectedCalls)
    {
        logger.VerifyLog(level, (m, _) => m.ToString().Contains(message), expectedCalls);
    }
}
