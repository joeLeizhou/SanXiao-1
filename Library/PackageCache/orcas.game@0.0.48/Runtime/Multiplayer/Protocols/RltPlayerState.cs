using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    [UnityEngine.Scripting.Preserve]
    public abstract class RltPlayerState : IRltProto
    {
        public ushort ID { get; set; }
        public int PlayerId { get; set; }
        public PlayerRoomState RoomState { get; set; }
        public PlayerNetState NetState { get; set; }

        public virtual void Deal()
        {

        }
    }

    public class DefaultRltPlayerState : RltPlayerState
    {
        
    }
}