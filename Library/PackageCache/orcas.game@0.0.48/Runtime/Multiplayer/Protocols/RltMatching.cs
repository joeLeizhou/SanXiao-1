using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    /// <summary>
    /// 多人匹配时，用于服务端通知所有人进入了matching的状态
    /// </summary>
    public abstract class RltMatching : IRltProto
    {
        public ushort ID { get; set; }
        public virtual void Deal()
        {
        }
    }
}