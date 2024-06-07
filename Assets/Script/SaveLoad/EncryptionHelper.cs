using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private static string PASSWORD = "tHisIstesT4tistory";
    private static readonly string KEY = PASSWORD.Substring(0, 128 / 8); // 16 characters

    public static string EncryptString(string plain)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plain);

        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.KeySize = 128;

            rijndael.GenerateIV(); // Generate a new IV for each encryption
            byte[] iv = rijndael.IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(iv, 0, iv.Length); // Prepend IV to the ciphertext

                ICryptoTransform encryptor = rijndael.CreateEncryptor(Encoding.UTF8.GetBytes(KEY), iv);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                byte[] encryptBytes = memoryStream.ToArray();
                return Convert.ToBase64String(encryptBytes);
            }
        }
    }

    public static string DecryptString(string encrypt)
    {
        byte[] encryptBytes = Convert.FromBase64String(encrypt);

        using (RijndaelManaged rijndael = new RijndaelManaged())
        {
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
            rijndael.KeySize = 128;

            using (MemoryStream memoryStream = new MemoryStream(encryptBytes))
            {
                byte[] iv = new byte[rijndael.BlockSize / 8];
                memoryStream.Read(iv, 0, iv.Length); // Extract the IV from the ciphertext

                ICryptoTransform decryptor = rijndael.CreateDecryptor(Encoding.UTF8.GetBytes(KEY), iv);
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    byte[] plainBytes = new byte[encryptBytes.Length - iv.Length];
                    int plainCount = cryptoStream.Read(plainBytes, 0, plainBytes.Length);
                    return Encoding.UTF8.GetString(plainBytes, 0, plainCount);
                }
            }
        }
    }
}