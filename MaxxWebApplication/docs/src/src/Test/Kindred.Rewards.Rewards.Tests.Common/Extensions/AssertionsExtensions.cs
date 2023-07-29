using FluentAssertions;

using JetBrains.Annotations;

namespace Kindred.Rewards.Rewards.Tests.Common.Extensions;

public static class AssertionsExtensions
{
    [NotNull]
    public static TSubject ShouldNotBeNull<TSubject>([CanBeNull] this TSubject source)
    {
        source.Should().NotBeNull();

        // ReSharper disable once AssignNullToNotNullAttribute
        return source;
    }
}
