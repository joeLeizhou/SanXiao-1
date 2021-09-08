using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqReconnect : IReqProto
    {
        public ushort ID { get; set; } = MultiPlayerProtoId.ReqReconnect;
        public int UserId { get; set; }     // 玩家id
        public long RoomID { get; set; }    // 房间id
        public int CheckCode { get; set; }  // 校验码
        public RoomState RoomState { get; set; }    // 客户端保存房间状态
    }

    public class DefaultReqReconnect : ReqReconnect
    {

    }
}