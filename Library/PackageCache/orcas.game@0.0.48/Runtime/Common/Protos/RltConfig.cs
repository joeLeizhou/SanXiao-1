using Orcas.Core;
using Orcas.Networking;
using UnityEngine.Scripting;

namespace Orcas.Game.Common
{
    public enum ConfigType : byte
    {
        Csv = 0,
        Json = 1
    }
    
    [Preserve]
    public class Config
    {
        [Preserve]
        public string FileName { get; set; }
        [Preserve]
        public byte Type { get; set; }
        [Preserve]
        public BigString Context { get; set; }
    }
    [Preserve]
    public class RltConfig : IRltProto
    {
        [Preserve]
        public ushort ID { get; set; }
        [Preserve]
        public Config[] Configs { get; set; }

        public void Deal()
        {
        }
    }
}