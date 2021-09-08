namespace Orcas.Networking
{
    public sealed class ChannelContext
    {
        public ushort Id;
        public bool Compelete;
        public bool Jump2Next;
    }
    
    public abstract class ChannelHandler<T1, T2> : IChannelHandler

    {
        public Channel Channel { get; set; }

        protected virtual void Server2Client(ChannelContext context, out T1 data1, in T2 data2)
        {
            data1 = default;
            context.Jump2Next = true;
        }

        protected virtual void Client2Server(ChannelContext context, in T1 data1, out T2 data2)
        {
            data2 = default;
            context.Jump2Next = true;
        }

        public void ServerToClient(ChannelContext context, out object data1, in object data2)
        {
            if (data2 is T2)
            {
                var d2 = (T2) data2;
                context.Jump2Next = false;
                Server2Client(context, out var d1, in d2);
                if (!context.Jump2Next)
                {
                    data1 = d1;
                }
                else
                {
                    data1 = data2;
                }
            }
            else
            {
                data1 = data2;
            }
        }

        public void ClientToServer(ChannelContext context, in object data1, out object data2)
        {
            if (data1 is T1)
            {
                var d1 = (T1) data1;
                context.Jump2Next = false;
                Client2Server(context, in d1, out var d2);
                if (!context.Jump2Next)
                {
                    data2 = d2;
                }
                else
                {
                    data2 = data1;
                }
            }
            else
            {
                data2 = data1;
            }
        }
    }
}