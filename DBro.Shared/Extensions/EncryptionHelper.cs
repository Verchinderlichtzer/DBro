using System.Security.Cryptography;

namespace DBro.Shared.Extensions;

public static class EncryptionHelper
{
    private static readonly byte[] _key =
    [
        0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF,
        0xFE, 0xDC, 0xBA, 0x98, 0x76, 0x54, 0x32, 0x10,
        0x1A, 0x2B, 0x3C, 0x4D, 0x5E, 0x6F, 0x7A, 0x8B,
        0x9C, 0xAD, 0xBE, 0xCF, 0xD0, 0xE1, 0xF2, 0x33
    ];

    private static readonly byte[] _iV =
    [
        0x98, 0x76, 0x54, 0x32, 0x10, 0xAB, 0xCD, 0xEF,
        0x12, 0x34, 0x56, 0x78, 0x90, 0xFE, 0xDC, 0xBA
    ];

    public static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iV;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }
        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iV;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream msDecrypt = new(Convert.FromBase64String(cipherText));
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);
        return srDecrypt.ReadToEnd();
    }
}