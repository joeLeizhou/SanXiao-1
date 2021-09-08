using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqStartGame : IReqProto
    {
        public ushort ID { get; set; } = MultiPlayerProtoId.ReqStartGame;
    }

    public class DefaultReqStartGame : ReqStartGame
    {
        
    }
}