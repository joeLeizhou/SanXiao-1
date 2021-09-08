namespace Orcas.Networking
{
    public interface IChannelHandler
    {
        Channel Channel { get; set; }
        void ServerToClient(ChannelContext context, out object data1, in object data2);
        void  ClientToServer(ChannelContext context, in object data1, out object data2);
    }
}