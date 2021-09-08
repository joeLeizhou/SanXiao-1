using System;

namespace Orcas.Networking
{
    public class AddHeaderChannelHandler : ChannelHandler<byte[], byte[]>
    {
        private const int PROTO_HEADER_SIZE = 4;
        private const int PROTO_ID_SIZE = 2;

        protected override void Client2Server(ChannelContext context, in byte[] data1, out byte[] data2)
        {
            var bytesLen = BitConverter.GetBytes(data1.Length + PROTO_ID_SIZE);
            var bytesId = BitConverter.GetBytes(context.Id);
            data2 = new byte[PROTO_HEADER_SIZE + PROTO_ID_SIZE + data1.Length];
            Buffer.BlockCopy(bytesLen, 0, data2, 0, PROTO_HEADER_SIZE);
            Buffer.BlockCopy(bytesId, 0, data2, PROTO_HEADER_SIZE, PROTO_ID_SIZE);
            Buffer.BlockCopy(data1, 0, data2, PROTO_HEADER_SIZE + PROTO_ID_SIZE, data1.Length);
        }
    }
}