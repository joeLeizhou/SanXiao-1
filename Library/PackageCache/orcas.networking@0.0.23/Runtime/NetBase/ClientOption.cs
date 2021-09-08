using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public class ClientOption
    {
        
        /// <summary>
        /// 超时时间，单位为秒
        /// </summary>
        public int TimeOut;

        /// <summary>
        /// 是否需要心跳
        /// </summary>
        public bool NeedHeartBeat;
        
        
        /// <summary>
        /// 心跳间隔，单位为秒
        /// </summary>
        public int HeartBeatInterval;
        
        /// <summary>
        /// 客户端上行心跳协议的ID
        /// </summary>
        public ushort ReqHeartBeatID;
        
        
        /// <summary>
        /// 服务器下行心跳协议的ID
        /// </summary>
        public ushort RltHeartBeatID;

        /// <summary>
        /// 是否需要自动尝试重连
        /// </summary>
        public bool NeedAutoReconnect;

        /// <summary>
        /// 如果需要自动尝试重连，自动尝试重连次数
        /// </summary>
        public int MaxReconnectCount;

        
        /// <summary>
        /// 断线重连是否以超时的形式控制。如果为true，MaxReconnectCount将失效
        /// </summary>
        public bool NeedAutoReconnectTimeOut;

        /// <summary>
        /// 断线重连的超时时间，单位为秒
        /// </summary>
        public int ReconnectTimeOut;


        public int BufferSize;

        #region Tcp参数

        /// <summary>
        /// 接受数据线程休眠间隔，毫秒
        /// </summary>
        public int ThreadSleepTime;

        #endregion


        #region Kcp参数
        public bool Nodelay;
        public bool UdpOpenWhile;
        public bool Crc32Check;
        public bool AckNoDelay;
        public bool Nc;
        public int Resend;
        public int Sndwnd;
        public int Rcvwnd;
        public int Mtu;
        public int MinRto;
        public int Conv;
        #endregion
        
        public ClientOption()
        {
            TimeOut = 15;
            NeedHeartBeat = false;
            HeartBeatInterval = 10;
            ReqHeartBeatID = 19000;
            RltHeartBeatID = 19001;
            ThreadSleepTime = 20;
            Nodelay = true;
            Resend = 2;
            Nc = true;
            Sndwnd = 256;
            Rcvwnd = 256;
            UdpOpenWhile = true;
            Crc32Check = false;
            AckNoDelay = true;
            Mtu = 1400;
            MinRto = 30;
            Conv = 0;
            NeedAutoReconnect = true;
            MaxReconnectCount = 3;
            NeedAutoReconnectTimeOut = false;
            ReconnectTimeOut = 20;
            BufferSize = 65536;
        }

        public static ClientOption Clone(ClientOption obj)
        {
            if (obj == null) return null;
            ClientOption option = new ClientOption();
            option.TimeOut = obj.TimeOut;
            option.NeedHeartBeat = obj.NeedHeartBeat;
            option.HeartBeatInterval = obj.HeartBeatInterval;
            option.ReqHeartBeatID = obj.ReqHeartBeatID;
            option.RltHeartBeatID = obj.RltHeartBeatID;
            option.ThreadSleepTime = obj.ThreadSleepTime;
            option.Nodelay = obj.Nodelay;
            option.Resend = obj.Resend;
            option.Nc = obj.Nc;
            option.Sndwnd = obj.Sndwnd;
            option.Rcvwnd = obj.Rcvwnd;
            option.UdpOpenWhile = obj.UdpOpenWhile;
            option.Crc32Check = obj.Crc32Check;
            option.AckNoDelay = obj.AckNoDelay;
            option.Mtu = obj.Mtu;
            option.MinRto = obj.MinRto;
            option.Conv = obj.Conv;
            option.NeedAutoReconnect = obj.NeedAutoReconnect;
            option.MaxReconnectCount = obj.MaxReconnectCount;
            option.NeedAutoReconnectTimeOut = obj.NeedAutoReconnectTimeOut;
            option.ReconnectTimeOut = obj.ReconnectTimeOut;
            option.BufferSize = obj.BufferSize;
            return option;
        }

        public void CopyFrom(ClientOption obj)
        {
            if (obj == null) return;
            this.TimeOut = obj.TimeOut;
            this.NeedHeartBeat = obj.NeedHeartBeat;
            this.HeartBeatInterval = obj.HeartBeatInterval;
            this.ReqHeartBeatID = obj.ReqHeartBeatID;
            this.RltHeartBeatID = obj.RltHeartBeatID;
            this.ThreadSleepTime = obj.ThreadSleepTime;
            this.Nodelay = obj.Nodelay;
            this.Resend = obj.Resend;
            this.Nc = obj.Nc;
            this.Sndwnd = obj.Sndwnd;
            this.Rcvwnd = obj.Rcvwnd;
            this.UdpOpenWhile = obj.UdpOpenWhile;
            this.Crc32Check = obj.Crc32Check;
            this.AckNoDelay = obj.AckNoDelay;
            this.Mtu = obj.Mtu;
            this.MinRto = obj.MinRto;
            this.Conv = obj.Conv;
            this.NeedAutoReconnect = obj.NeedAutoReconnect;
            this.MaxReconnectCount = obj.MaxReconnectCount;
            this.NeedAutoReconnectTimeOut = obj.NeedAutoReconnectTimeOut;
            this.ReconnectTimeOut = obj.ReconnectTimeOut;
            this.BufferSize = obj.BufferSize;
        }
    }    
}

