namespace Orcas.Networking
{
    public class Proto2BytesChannelHandler : ChannelHandler<IProtocol, byte[]>
    {
        protected override void Client2Server(ChannelContext context, in IProtocol data1, out byte[] data2)
        {
            context.Id = data1.ID;
            if (ProtocolFactory.Instance.CheckIsLuaProto(context.Id) == false)
            {
                data2 = ProtocolFactory.Instance.GetCSProtocolBytes(data1);
            }
            else
            {
                data2 = ProtocolFactory.Instance.GetLuaProtocolBytes(data1 as ReqLuaMessage);
            }
        }
    }
}