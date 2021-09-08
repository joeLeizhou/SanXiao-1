using Orcas.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orcas.Networking
{
    [UnityEngine.Scripting.Preserve]
    public abstract class ReqHeartBeat : IReqProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
    }
    [UnityEngine.Scripting.Preserve]
    public class DefaultReqHeartBeat : ReqHeartBeat
    {

    }
}
