using System.Security.Cryptography;
using System.Text;

namespace Template.Utility.Extensions;

public static class StringExtension
{
    public static string GenerateRandomString(int length)
    {
        using (var crypto = RandomNumberGenerator.Create())
        {
            var bits = length * 6;
            var byte_size = (bits + 7) / 8;
            var bytesarray = new byte[byte_size];
            crypto.GetBytes(bytesarray);
            return Convert.ToBase64String(bytesarray);
        }
    }

    public static string HashHMACSHA256(this string signature, string? key = null)
    {
        if (string.IsNullOrEmpty(signature))
        {
            return string.Empty;
        }

        using (var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(key ?? string.Empty)))
        {
            var signatureByted = hasher.ComputeHash(Encoding.UTF8.GetBytes(signature));
            return Convert.ToBase64String(signatureByted);
        }
    }

    public static bool IsHashHMACSHA256Match(this string signature, string expectedHashValue, string? key = null)
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(expectedHashValue))
        {
            return false;
        }

        var hashValues = signature.HashHMACSHA256(key);

        return string.Equals(expectedHashValue, hashValues);
    }
}