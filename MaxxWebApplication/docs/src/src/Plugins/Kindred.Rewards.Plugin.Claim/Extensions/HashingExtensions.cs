using System.Security.Cryptography;
using System.Text;

namespace Kindred.Rewards.Plugin.Claim.Extensions;
public static class HashingExtensions
{
    public static string GetSha256(this string item)
    {
        var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(item);
        var hash = sha.ComputeHash(bytes);

        return Convert.ToBase64String(hash);
    }
}
