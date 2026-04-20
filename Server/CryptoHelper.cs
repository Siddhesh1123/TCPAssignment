using System.Security.Cryptography;
using System.Text;

namespace Server  
{
    public static class CryptoHelper
    {
        // Both client and server must use the same key
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("A1B2C3D4E5F6G7H8"); // 16 bytes = AES-128
        private static readonly byte[] IV  = Encoding.UTF8.GetBytes("H8G7F6E5D4C3B2A1"); // 16 bytes

        public static string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            var encryptor = aes.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        public static string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            var decryptor = aes.CreateDecryptor();
            byte[] inputBytes = Convert.FromBase64String(cipherText);
            byte[] decrypted = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}