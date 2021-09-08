using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    [UnityEngine.Scripting.Preserve]
    public abstract class RltRoundStart : IRltProto
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
    public class DefaultRltRoundStart : RltRoundStart
    {

    }
}