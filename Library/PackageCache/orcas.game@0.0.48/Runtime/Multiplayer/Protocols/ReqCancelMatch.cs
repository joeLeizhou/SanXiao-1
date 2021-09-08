using Orcas.Game.Common;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqCancelMatch : IReqProto
    {
        public ushort ID { get; set; } = CommonProtoId.ReqCancelMatch;
        public short MatchId { get; set; }
    }

    public class DefaultReqCancelMatch : ReqCancelMatch
    {
        
    }
}