using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    [UnityEngine.Scripting.Preserve]
    public class ReqTestSpeed : IReqProto
    {
        public ushort ID { get; set; }
        public bool IsEnd { get; set; }
    }
}
