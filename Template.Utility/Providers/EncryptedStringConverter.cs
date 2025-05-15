using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Template.Utility.Providers;

public class EncryptedStringConverter(string encryptionKey) : ValueConverter<string, string>(
        v => CryptoProvider.Encrypt(v, encryptionKey),
        v => CryptoProvider.Decrypt(v, encryptionKey)) { }