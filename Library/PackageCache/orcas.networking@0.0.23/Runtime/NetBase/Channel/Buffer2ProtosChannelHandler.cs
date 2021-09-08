namespace Orcas.Networking
{
    public class Buffer2ProtosChannelHandler : ChannelHandler<IRltProto[], ByteBuffer>
    {
        protected override void Server2Client(ChannelContext context, out IRltProto[] data1, in ByteBuffer data2)
        {
            data1 = data2.ReadProtocol();
            if (data1 == null || data1.Length == 0)
            {
                context.Jump2Next = true;
            }
        }
    }
}