using System;
using System.IO;

namespace Orcas.AssetBuilder
{
    public static class SecurityUtils
    {
        private static byte[] E1(byte[] bytes, char[] key)
        {
            int keyCount = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= (byte)key[keyCount++];
                keyCount %= key.Length;
            }
            return bytes;
        }

        public static byte[] Encrypt(byte[] data, char[] key)
        {
            return E1(data, key);
        }

        public static byte[] Decrypt(byte[] data, char[] key)
        {
            return E1(data, key);
        }

        public static void Encrypt(string fullPath, char[] key)
        {
            var bytes = File.ReadAllBytes(fullPath);
            bytes = Encrypt(bytes, key);
            File.WriteAllBytes(fullPath, bytes);
        }

        public static void Decrypt(string fullPath, char[] key)
        {
            var bytes = File.ReadAllBytes(fullPath);
            bytes = Decrypt(bytes, key);
            File.WriteAllBytes(fullPath, bytes);
        }
    }
}
