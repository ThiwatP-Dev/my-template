using System.Security.Cryptography;
using System.Text;

namespace Template.Core.Extensions;

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
}