using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqEnterRoom : IReqProto
    {
        public ushort ID { get; set; } = MultiPlayerProtoId.ReqEnterRoom;
        public int PlayerID { get; set; }    // 玩家ID
        public long RoomID { get; set; }     // 房间ID
        public int CheckCode { get; set; }   // 校验码
    }

    public class DefaultReqEnterRoom : ReqEnterRoom
    {
        
    }
}