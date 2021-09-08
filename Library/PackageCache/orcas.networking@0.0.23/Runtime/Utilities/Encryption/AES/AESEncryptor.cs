using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Orcas.Networking.Utilities
{
    public class AESEncryptor : IEncryptor
    {
        RijndaelManaged _cipher;
        string _key = "";
        bool _useRandomIV = true;
        bool _useSHA1WhenSetKey = true;

        public void UseRandomIV(bool value)
        {
            _useRandomIV = value;
            if (value == false)
            {
                UseZeroIV();
            }
        }

        public void UseSHA1WhenSetKey(bool value)
        {
            _useSHA1WhenSetKey = value;
        }

        public AESEncryptor()
        {
            _cipher = new RijndaelManaged();
            _cipher.Mode = CipherMode.CBC;
            _cipher.Padding = PaddingMode.PKCS7;
        }

        public AESEncryptor(string key)
        {
            _cipher = new RijndaelManaged();
            _cipher.Mode = CipherMode.CBC;
            _cipher.Padding = PaddingMode.PKCS7;
            _key = key;
        }

        public void SetCipherMode(CipherMode mode)
        {
            _cipher.Mode = mode;
        }

        public void SetPaddingMode(PaddingMode mode)
        {
            _cipher.Padding = mode;
        }
        private byte[] GetKeyBytes()
        {
            if (_key == "")
            {
                Debug.Log("Key is null");
                return null;
            }

            var keyBytes = Encoding.UTF8.GetBytes(_key);

            return _useSHA1WhenSetKey ? EncryptKeyWithSHA1(keyBytes) : keyBytes;       
        }

        private byte[] EncryptKeyWithSHA1(byte[] key)
        {
            var sha = new SHA1CryptoServiceProvider();

            var hash = sha.ComputeHash(key);

            byte[] encryptedKey = new byte[16];
            Array.Copy(hash, 0, encryptedKey, 0, 16);
            return encryptedKey;
        }

        public byte[] Encrypt(byte[] content)
        {
            _cipher.Key = GetKeyBytes();
            
            return _useRandomIV ? EncryptWithRandomIV(content) : EncryptWithZeroIV(content);
        }

        private byte[] EncryptWithRandomIV(byte[] content)
        {
            _cipher.GenerateIV();
            var iv = _cipher.IV;
            var encryptor = _cipher.CreateEncryptor();
            var encodedBytes = encryptor.TransformFinalBlock(content, 0, content.Length);
            var packedBytes = Pack(iv, encodedBytes);
            return packedBytes;
        }

        private byte[] EncryptWithZeroIV(byte[] content)
        {
            var encryptor = _cipher.CreateEncryptor();
            var encodedBytes = encryptor.TransformFinalBlock(content, 0, content.Length);
            return encodedBytes;
        }

        public byte[] Decrypt(byte[] packedBytes)
        {
            _cipher.Key = GetKeyBytes();
            return _useRandomIV ? DecryptPackedBytes(packedBytes) : DecryptBytes(packedBytes);      
        }

        private byte[] DecryptPackedBytes(byte[] packedBytes)
        {
            (var iv, var encodedBytes) = UnPack(packedBytes);

            _cipher.Key = GetKeyBytes();
            _cipher.IV = iv;
            var decryptor = _cipher.CreateDecryptor();
            var content = decryptor.TransformFinalBlock(encodedBytes, 0, encodedBytes.Length);
            return content;
        }

        private byte[] DecryptBytes(byte[] bytes)
        {
            var decryptor = _cipher.CreateDecryptor();
            var content = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);
            return content;
        }

        private byte[] Pack(byte[] iv, byte[] encodedBytes)
        {
            int l = 16 + encodedBytes.Length;
            var packedBytes = new byte[l];
            Array.Copy(iv, 0, packedBytes, 0, 16);
            Array.Copy(encodedBytes, 0, packedBytes, 16, encodedBytes.Length);
            return packedBytes;
        }

        private (byte[] iv, byte[] cipherBytes) UnPack(byte[] packedBytes)
        {
            var iv = new byte[16];
            Array.Copy(packedBytes, 0, iv, 0, 16);
            int l = packedBytes.Length - 16;
            var encodedBytes = new byte[l];
            Array.Copy(packedBytes, 16, encodedBytes, 0, l);
            return (iv, encodedBytes);
        }

        public string GetKey()
        {
            return _key;
        }

        public void SetKey(string key)
        {
            _key = key;
        }

        private void UseZeroIV()
        {
            byte[] iv = Encoding.UTF8.GetBytes("0000000000000000");
            _cipher.IV = iv;
        }
    }
}