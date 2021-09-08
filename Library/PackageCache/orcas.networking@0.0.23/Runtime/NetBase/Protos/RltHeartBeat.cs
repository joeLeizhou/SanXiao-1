using Orcas.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orcas.Networking
{
    [UnityEngine.Scripting.Preserve]
    public abstract class RltHeartBeat : IRltProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
        [UnityEngine.Scripting.Preserve]
        public long Time { get; set; }
        public void Deal()
        {

        }
    }
    [UnityEngine.Scripting.Preserve]
    public class DefaultRltHeartBeat : RltHeartBeat
    {
    }

}
