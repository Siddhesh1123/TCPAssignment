using System.Security.Cryptography;
using System.Text;
using Server.Domain.Interfaces;

namespace Server.Infrastructure.Crypto
{
    public class CryptoService : ICryptoService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("A1B2C3D4E5F6G7H8");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("H8G7F6E5D4C3B2A1");

        public string Encrypt(string plainText)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                    throw new ArgumentException("Plain text cannot be null or empty");

                using var aes = Aes.Create();
                aes.Key = Key;
                aes.IV = IV;

                var encryptor = aes.CreateEncryptor();
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Encryption failed: {ex.Message}", ex);
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = Key;
                aes.IV = IV;

                var decryptor = aes.CreateDecryptor();
                byte[] inputBytes = Convert.FromBase64String(cipherText);
                byte[] decrypted = decryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Invalid Base64 format in encrypted data", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Decryption failed: {ex.Message}", ex);
            }
        }
    }
}
