using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    [UnityEngine.Scripting.Preserve]
    public class RltRenterGame : IRltProto
    {
        [PropertyIndex(0)]
        public ushort ID { get; set; }
        [PropertyIndex(1)]
        public byte Round { get; set; }
        [PropertyIndex(2)]
        public int PlayerID { get; set; }

        public void Deal()
        {
        }
    }

    public class DefaultRltRenterGame : RltRenterGame
    {

    }
}