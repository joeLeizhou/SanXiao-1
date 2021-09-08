using System;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    [UnityEngine.Scripting.Preserve]
    public abstract class RltMatch : IRltProto
    {
        public ushort ID { get; set; }
        public short Code { get; set; }

        public virtual void Deal()
        {

        }
    }

    public class DefaultRltMatch : RltMatch
    {

    }
}