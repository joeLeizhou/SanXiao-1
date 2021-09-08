using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    [Serializable]
    public class IPAddressInfo
    {
        /// <summary>
        /// 服务器ID（多服务器情况下）
        /// </summary>
        public int ServerId;
        
        /// <summary>
        /// 类型，对应ConnectType枚举
        /// </summary>
        public byte Type;
        
        /// <summary>
        /// 域名或者IP
        /// </summary>
        public string Host;
        
        /// <summary>
        /// 端口
        /// </summary>
        public int Port;
        
        /// <summary>
        /// 测速过程中，花费的平均时间
        /// </summary>
        public float AvgTime;
        
        /// <summary>
        /// 连接失败次数
        /// </summary>
        public int FailCount;
    }
}
