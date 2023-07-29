namespace Kindred.Rewards.Rewards.Tests.Common.Extensions;

public static class LoopUtilities
{
    public static List<TItem> CreateMany<TItem>(Func<TItem> action, int count = 3)
    {
        var list = new List<TItem>();
        for (var i = 0; i < count; i++)
        {
            list.Add(action());
        }

        return list;
    }
}
