using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Folder_locker
{
    internal class AESHelper
    {
        private static readonly string Key = "Asbcasdjffhaiweaksjdhwrsd"; // Change this to a secure key

        public static void EncryptFile(string filePath)
        {
            byte[] keyBytes = new byte[32];
            using (SHA256 sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Key)); // Hash key to get 32 bytes
            }
            
            byte[] ivBytes = keyBytes.Take(16).ToArray(); // IV must be 16 bytes

            byte[] fileBytes = File.ReadAllBytes(filePath);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);
                    File.WriteAllBytes(filePath + ".locked", encryptedBytes);
                }
            }
            File.Delete(filePath); // Delete the original file
        }

        public static void DecryptFile(string filePath)
        {
            byte[] keyBytes = new byte[32];
            using (SHA256 sha256 = SHA256.Create())
            {
                keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Key)); // Hash key to get 32 bytes
            }
            byte[] ivBytes = keyBytes.Take(16).ToArray();

            byte[] fileBytes = File.ReadAllBytes(filePath);
            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);
                    File.WriteAllBytes(filePath.Replace(".locked", ""), decryptedBytes);
                }
            }
            File.Delete(filePath); // Delete the encrypted file
        }
    }
}
