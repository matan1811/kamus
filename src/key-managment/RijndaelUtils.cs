﻿using System.IO;
using System.Security.Cryptography;

namespace Kamus.KeyManagement
{
    public static class RijndaelUtils
    {
        public static byte[] GenerateKey(int keySize = 32)
        {
            using (var aes = new AesManaged())
            {
                aes.KeySize = 32;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public static (byte[] encryptedData, byte[] iv) Encrypt(byte[] key, byte[] data)
        {
            byte[] iv = GetRandomData(16);
            byte[] result;
            using (var rijndael = new RijndaelManaged())
            {
                using (var encryptor = rijndael.CreateEncryptor(key, iv))
                using (var resultStream = new MemoryStream())
                {
                    using (var rijndaelStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                    using (var plainStream = new MemoryStream(data))
                    {
                        plainStream.CopyTo(rijndaelStream);
                    }

                    result = resultStream.ToArray();
                }
            }

            return (result, iv);
        }

        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] encryptedData)
        {
            byte[] result;
            using (var rijndael = new RijndaelManaged())
            {
                using (var decryptor = rijndael.CreateDecryptor(key, iv))
                using (var resultStream = new MemoryStream())
                {
                    using (var rijndaelStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
                    using (var cipherStream = new MemoryStream(encryptedData))
                    {
                        cipherStream.CopyTo(rijndaelStream);
                    }

                    result = resultStream.ToArray();
                }
            }

            return result;

        }

        private static byte[] GetRandomData(int size)
        {
            var provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[size];
            provider.GetBytes(byteArray);
            return byteArray;
        }
    }
}
