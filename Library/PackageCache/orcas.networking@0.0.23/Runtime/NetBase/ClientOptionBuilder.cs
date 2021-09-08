using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public class ClientOptionBuilder
    {
        private ClientOption _option;

        public ClientOptionBuilder(ConnectType type)
        {
            _option = new ClientOption();
            // 根据不同的类型的连接给出一部分在其他项目经过验证的经验属性值
            // 心跳协议暂时搞成写死的，不想依赖Game包，如需要，传自行定义的Option
            switch (type)
            {
                case ConnectType.Http:
                case ConnectType.Https:
                    SetTimeOut(10);
                    break;
                case ConnectType.Kcp:
                    SetHeartBeatParams(true, 1, 10, 1009, 1010);
                    break;
                case ConnectType.Tcp:
                    SetHeartBeatParams(true, 6, 15, 1009, 1010);
                    break;
            }            
        }
        
        public ClientOptionBuilder SetTimeOut(int timeout)
        {
            _option.TimeOut = timeout;
            return this;
        }
        
        public ClientOptionBuilder SetNeedHeartBeat(bool needHeartbeat)
        {
            _option.NeedHeartBeat = needHeartbeat;
            return this;
        }
        
        public ClientOptionBuilder SetHeartBeatInterval(int inv)
        {
            _option.HeartBeatInterval = inv;
            return this;
        }
        
        public ClientOptionBuilder SetReqHeartBeatID(ushort id)
        {
            _option.ReqHeartBeatID = id;
            return this;
        }
        
        public ClientOptionBuilder SetRltHeartBeatID(ushort id)
        {
            _option.RltHeartBeatID = id;
            return this;
        }
        
        public ClientOptionBuilder SetThreadSleepTime(int t)
        {
            _option.ThreadSleepTime = t;
            return this;
        }
        
        public ClientOptionBuilder SetNodelay(bool nodelay)
        {
            _option.Nodelay = nodelay;
            return this;
        }
        
        public ClientOptionBuilder SetResend(int resnd)
        {
            _option.Resend = resnd;
            return this;
        }
        
        public ClientOptionBuilder SetNc(bool nc)
        {
            _option.Nc = nc;
            return this;
        }
        
        public ClientOptionBuilder SetSndwnd(int sndwnd)
        {
            _option.Sndwnd = sndwnd;
            return this;
        }
        
        public ClientOptionBuilder SetRcvwnd(int rcvwnd)
        {
            _option.Rcvwnd = rcvwnd;
            return this;
        }

        public ClientOptionBuilder SetMtu(int value)
        {
            _option.Mtu = value;
            return this;
        }
        
        public ClientOptionBuilder SetMinRto(int value)
        {
            _option.MinRto = value;
            return this;
        }
        
        public ClientOptionBuilder SetConv(int value)
        {
            _option.Conv = value;
            return this;
        }
        
        public ClientOptionBuilder SetMaxReconnectCount(int value)
        {
            _option.MaxReconnectCount = value;
            return this;
        }
        
        public ClientOptionBuilder SetReconnectTimeOut(int value)
        {
            _option.MaxReconnectCount = value;
            return this;
        }

        public ClientOptionBuilder SetUdpOpenWhile(bool value)
        {
            _option.UdpOpenWhile = value;
            return this;
        }
        
        public ClientOptionBuilder SetCrc32Check(bool value)
        {
            _option.Crc32Check = value;
            return this;
        }

        
        public ClientOptionBuilder SetAckNoDelay(bool value)
        {
            _option.AckNoDelay = value;
            return this;
        }
        
        public ClientOptionBuilder SetNeedAutoReconnect(bool value)
        {
            _option.NeedAutoReconnect = value;
            return this;
        }
        
        public ClientOptionBuilder SetNeedAutoReconnectTimeOut(bool value)
        {
            _option.NeedAutoReconnectTimeOut = value;
            return this;
        }
        
        public ClientOptionBuilder SetBufferSize(int size)
        {
            _option.BufferSize = size;
            return this;
        }
        
        public ClientOptionBuilder SetAutoReconnectParams(bool needReconnect, int reconnectTime, bool needReconnectTimeOut, int reconnectTimeout)
        {
            _option.NeedAutoReconnect = needReconnect;
            _option.MaxReconnectCount = reconnectTime;
            _option.NeedAutoReconnectTimeOut = needReconnectTimeOut;
            _option.ReconnectTimeOut = reconnectTimeout;
            return this;
        }
        
        public ClientOptionBuilder SetNodelayParams(bool nodelay, int resend, bool nc){
            _option.Nodelay = nodelay;
            _option.Resend = resend;
            _option.Nc = nc;
            return this;
        }

        public ClientOptionBuilder SetHeartBeatParams(bool needHeartBeat, int heartInterval, int heartTimeout, ushort reqId, ushort rltId)
        {
            _option.NeedHeartBeat = needHeartBeat;
            _option.HeartBeatInterval = heartInterval;
            _option.TimeOut = heartTimeout;
            _option.ReqHeartBeatID = reqId;
            _option.RltHeartBeatID = rltId;
            return this;
        }

        public ClientOptionBuilder SetHeartBeatProtocols(ushort reqId, ushort rltId)
        {
            _option.ReqHeartBeatID = reqId;
            _option.RltHeartBeatID = rltId;
            return this;
        }

        public ClientOptionBuilder SetWnd(int snd, int rcv)
        {
            _option.Sndwnd = snd;
            _option.Rcvwnd = rcv;
            return this;
        }

        public ClientOption Build()
        {
            return _option;
        }
    }    
}


