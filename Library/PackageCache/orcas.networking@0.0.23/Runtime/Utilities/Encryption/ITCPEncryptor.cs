using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking.Utilities
{
    public interface IEncryptor
    {
        byte[] Encrypt(byte[] content);
        byte[] Decrypt(byte[] content);
    }
}
