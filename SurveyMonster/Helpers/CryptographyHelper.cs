using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Lms.Shared.Domain.Helpers
{
    public static class CryptographyHelper
    {

       
        
        public static string Encrypt(string plainText, string key)
        {
            byte[] iv = new byte[16];
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
                writer.Write(plainText);

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static string Decrypt(string cipherText, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            using var memoryStream = new MemoryStream(buffer);
            using var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var reader = new StreamReader(cryptoStream);
            return reader.ReadToEnd();
        }

    }
}
