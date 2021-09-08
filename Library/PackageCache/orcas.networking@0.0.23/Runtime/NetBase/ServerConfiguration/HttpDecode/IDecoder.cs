using Orcas.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public interface IDecoder
    {
        IResponseData Decode(int responseCode, string responseStr);
    }
}
