using Orcas.Game.Common;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqMatch : IReqProto
    {
        public ushort ID { get; set; } = CommonProtoId.ReqMatch;
        public short MatchId { get; set; }  //客户端区分匹配请求的标识
        public byte MatchType { get; set; } //匹配类型
        public int Stage { get; set; }      //请求匹配的stage
    }

    public class DefaultReqMatch : ReqMatch
    {
        
    }
}