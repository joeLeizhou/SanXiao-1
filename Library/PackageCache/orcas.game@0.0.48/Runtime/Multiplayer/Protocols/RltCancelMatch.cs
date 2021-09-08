using System;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{

    [UnityEngine.Scripting.Preserve]
    public abstract class RltCancelMatch : IRltProto
    {
        public ushort ID { get; set; }
        public short MatchID { get; set; }
        public short Code { get; set; }

        public void Deal()
        {
        }

    }

    public class DefaultRltCancelMatch : RltCancelMatch
    {

    }
}
