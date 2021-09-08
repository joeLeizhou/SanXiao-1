﻿using Orcas.Networking;

namespace Orcas.Game.Common
{
    [UnityEngine.Scripting.Preserve]
    public class ReqBindAccount : IReqProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
        /// <summary>
        /// 类型 1:绑定 2:切换
        /// </summary>
        /// <value></value>
        [UnityEngine.Scripting.Preserve]
        public byte Type { get; set; }
        /// <summary>
        /// 登录类型
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public LoginType LoginType { get; set; }
        /// <summary>
        /// 账户ID
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string AccountId { get; set; }
        /// <summary>
        /// 类似于验证码一类的东西
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string Code { get; set; }
    }
}