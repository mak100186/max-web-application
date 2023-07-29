using AutoFixture;

using Kindred.Rewards.Core.Models;

namespace Kindred.Rewards.Rewards.Tests.Common.DataBuilders;

public class DataBuilderBase
{
    protected DataBuilderBase()
    {
    }

    protected static IFixture Fixture => new Fixture
    {
        Behaviors = { new OmitOnRecursionBehavior() }
    };


    protected static Action<T> SetCallbackIfNull<T>(Action<T> callback)
        where T : RewardDomainModel
    {
        callback ??= _ => { };
        return callback;
    }
}
