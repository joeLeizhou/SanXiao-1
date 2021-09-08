using System.Collections.Generic;

namespace Orcas.Networking
{
    public sealed class Channel
    {
        private List<IChannelHandler> _channels;
        public IClient Client { get; internal set; }

        public Channel()
        {
            _channels = new List<IChannelHandler>();
        }
        public Channel AddChannel(IChannelHandler channelHandler)
        {
            if (channelHandler == null) return this;
            channelHandler.Channel = this;
            _channels.Add(channelHandler);
            return this;
        }

        public Channel RemoveChannel(IChannelHandler channelHandler)
        {
            _channels.Remove(channelHandler);
            return this;
        }

        public Channel Clear()
        {
            _channels.Clear();
            return this;
        }

        public Channel RemoveAt(int index)
        {
            _channels.RemoveAt(index);
            return this;
        }

        internal void ServerToClient(ByteBuffer buffer)
        {
            object output;
            object input = buffer;
            var context = new ChannelContext();
            for (var i = _channels.Count - 1; i >= 0; i--)
            {
                _channels[i].ServerToClient(context, out output, in input);
                if (context.Compelete) break;
                input = output;
            }
        }
        
        internal byte[] ClientToServer(object protoData)
        {
            object output = null;
            var input = protoData;
            var context = new ChannelContext();
            for (var i = 0; i < _channels.Count; i++)
            {
                _channels[i].ClientToServer(context, in input, out output);
                if (context.Compelete) break;
                input = output;
            }

            return (byte[]) output;
        }
    }
}