namespace Kindred.Rewards.Core.Extensions;

public static class ObjectExtensions
{
    public static void ThrowIfNull(this object arg, string message = "")
    {
        if (arg == null)
        {
            throw new ArgumentNullException(message);
        }
    }
}
