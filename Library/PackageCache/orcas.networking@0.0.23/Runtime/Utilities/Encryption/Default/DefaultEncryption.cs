using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking.Utilities
{
    public class DefaultEncryption : IEncryptor
    {
        public byte[] Decrypt(byte[] content)
        {
            return content;
        }

        public byte[] Encrypt(byte[] content)
        {
            return content;
        }
    }
}
