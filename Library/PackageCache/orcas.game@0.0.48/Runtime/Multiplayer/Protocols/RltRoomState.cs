using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    [UnityEngine.Scripting.Preserve]
    public abstract class RltRoomState: IRltProto
    {
        public ushort ID { get; set; }
        public RoomState State { get; set; }
        public GameStateReason Reason { get; set; }

        public void Deal()
        {
        }
    }

    public class DefaultRltRoomState : RltRoomState
    {
        
    }
}