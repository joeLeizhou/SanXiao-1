using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Orcas.Networking
{
    public enum NetworkState
    {
        CONNECTED = 0,
        DISCONNECTED,
        CONNECTING
    }

    public abstract class ClientBase : IClient
    {
        public IClientEventHandler ClientEventHandler { get; set; }

        protected volatile NetworkState _networkState = NetworkState.DISCONNECTED;
        public NetworkState NetworkState { get { return _networkState; } protected set { _networkState = value; } }

        private Channel _channel;
        public Channel Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = new Channel();
                    _channel.Client = this;
                    _channel.AddChannel(new ProtoDistributeHandler())   
                        .AddChannel(new Proto2BytesChannelHandler())         
                        .AddChannel(new Buffer2ProtosChannelHandler())       
                        .AddChannel(new AddHeaderChannelHandler());          
                }

                return _channel;
            }
            set
            {
                value.Client = this;
                _channel = value;
            }
        }

        private ClientOption _option;

        public ClientOption Option
        {
            get { return _option; }
            set { _option = value; }
        }

        public abstract void SendMessage(IReqProto message);

        public abstract Task<bool> Connect();

        public abstract void DisConnect();   
        
        public void SetClientOption(ClientOption option)
        {
            Option = option;
        }
    }
}