namespace Kindred.Rewards.Rewards.Tests.Common.Extensions;

public static class RandomExtensionMethods
{
    /// <summary>
    ///     Returns a random long from min (inclusive) to max (exclusive)
    /// </summary>
    /// <param name="random">The given random instance</param>
    /// <param name="min">The inclusive minimum bound</param>
    /// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
    public static long NextLong(this Random random, long min, long max)
    {
        if (max <= min)
        {
            throw new ArgumentOutOfRangeException(nameof(max), "max must be > min!");
        }

        //Working with ulong so that modulo works correctly with values > long.MaxValue
        var uRange = (ulong)(max - min);

        ulong ulongRand;
        do
        {
            var buf = new byte[8];
#pragma warning disable CA5394 // Do not use insecure randomness
            random.NextBytes(buf);
#pragma warning restore CA5394 // Do not use insecure randomness
            ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
        }
        while (ulongRand > ulong.MaxValue - (ulong.MaxValue % uRange + 1) % uRange);

        return (long)(ulongRand % uRange) + min;
    }
}

public static class RandomLongGenerator
{
    private static readonly Random s_random = new();

    public static long Create()
    {
        return s_random.NextLong(0, 1234567890L);
    }

    public static long Create(long from, long to)
    {
        return s_random.NextLong(from, to);
    }
}
