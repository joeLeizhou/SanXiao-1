using Orcas.Core.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public class DefaultResponseData :IResponseData
    {
        public int Code { get; set; }
        public string DataStr { get; set; }

        public DefaultResponseData(int Code, string DataStr)
        {
            this.Code = Code;
            this.DataStr = DataStr;
        }
        /*
        public IResponseData Deserialize(int code, string dataStr)
        {
            return new DefaultResponseData(code, dataStr);
        }
        */
    }
}
