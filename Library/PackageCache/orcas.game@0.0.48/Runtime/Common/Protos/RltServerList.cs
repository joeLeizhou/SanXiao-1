using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orcas.Networking;

namespace Orcas.Game.Common
{
    [UnityEngine.Scripting.Preserve]
    public class ServerInfo
    {
        [UnityEngine.Scripting.Preserve]
        public string Host { get; set; }
        [UnityEngine.Scripting.Preserve]
        public int Port { get; set; }
        [UnityEngine.Scripting.Preserve]
        public bool IsTcp { get; set; }
    }

    [UnityEngine.Scripting.Preserve]
    public class ServerGroupInfo
    {
        [UnityEngine.Scripting.Preserve]
        public byte ServerID { get; set; }
        [UnityEngine.Scripting.Preserve]
        public ServerInfo[] Servers { get; set; }
    }

    [UnityEngine.Scripting.Preserve]
    public class RltServerList : IRltProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
        [UnityEngine.Scripting.Preserve]
        public ServerGroupInfo[] Groups { get; set; }

        public void Deal()
        {
        }
    }

}
