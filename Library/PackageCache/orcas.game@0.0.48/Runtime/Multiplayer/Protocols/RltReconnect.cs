using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public class RltReconnect : IRltProto
    {
        public ushort ID { get; set; }
        public short Code { get; set; }
        public RoomState RoomState { get; set; }
        public GameStateReason Reason { get; set; }
        public int Stage { get; set; }
        public int Scene { get; set; }
        public byte BattleType { get; set; }
        public void Deal()
        {
        }
    }

    public class DefaultRltReconnect : RltReconnect
    {

    }
}