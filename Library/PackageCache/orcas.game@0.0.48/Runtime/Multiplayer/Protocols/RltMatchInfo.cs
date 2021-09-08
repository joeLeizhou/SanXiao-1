using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    [UnityEngine.Scripting.Preserve]
    public abstract class RltMatchInfo : IRltProto
    {
        [PropertyIndex(0)]
        public ushort ID { get; set; }
        [PropertyIndex(1)]
        public byte ServerID { get; set; }
        [PropertyIndex(2)]
        public ConnectType ConnectType { get; set; }
        [PropertyIndex(3)]
        public string Host { get; set; }
        [PropertyIndex(4)]
        public int Port { get; set; }
        [PropertyIndex(5)]
        public long RoomID { get; set; }
        [PropertyIndex(6)]
        public int CheckCode { get; set; }
        [PropertyIndex(7)]
        public int Stage { get; set; }
        [PropertyIndex(8)]
        public int Scene { get; set; }
        [PropertyIndex(9)]
        public byte BattleType { get; set; }
        [PropertyIndex(10)]
        public int OpponentID { get; set; }
        [PropertyIndex(11)]
        public int AIID { get; set; }

        public virtual void Deal()
        {
        }
    }

    public class DefaultRltMatchInfo : RltMatchInfo
    {
        
    }
}
