using System.Security.Cryptography;
using System.Text;

namespace Template.Utility.Providers;

public static class CryptoProvider
{
    public static string Encrypt(this string plainText, string base64Key)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(base64Key);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        var iv = aes.IV;
        var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Embed IV (4 + cipher + 12)
        var result = new byte[4 + cipherBytes.Length + 12];
        Buffer.BlockCopy(iv, 0, result, 0, 4);
        Buffer.BlockCopy(cipherBytes, 0, result, 4, cipherBytes.Length);
        Buffer.BlockCopy(iv, 4, result, 4 + cipherBytes.Length, 12);

        return Convert.ToBase64String(result);
    }

    public static string Decrypt(this string cipherText, string base64Key)
    {
        var data = Convert.FromBase64String(cipherText);
        var iv = new byte[16];
        var cipherBytes = new byte[data.Length - 16];

        Buffer.BlockCopy(data, 0, iv, 0, 4);
        Buffer.BlockCopy(data, 4, cipherBytes, 0, cipherBytes.Length);
        Buffer.BlockCopy(data, 4 + cipherBytes.Length, iv, 4, 12);

        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String(base64Key);
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}