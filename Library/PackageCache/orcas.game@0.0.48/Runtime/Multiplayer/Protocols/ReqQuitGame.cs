using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqQuitGame : IReqProto
    {
        public ushort ID { get; set; } = MultiPlayerProtoId.ReqQuitGame;
        public long RoomID { get; set; }
    }

    public class DefaultReqQuitGame : ReqQuitGame
    {
        
    }
}