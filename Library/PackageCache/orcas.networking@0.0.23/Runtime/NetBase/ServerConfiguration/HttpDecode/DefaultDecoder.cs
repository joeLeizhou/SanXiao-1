using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Orcas.Networking
{
    public class DefaultDecoder : IDecoder
    {
        public IResponseData Decode(int responseCode, string dataStr)
        {
            return new DefaultResponseData(responseCode, dataStr);
        }
    }
}
