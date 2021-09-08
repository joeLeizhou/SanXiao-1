using System;
using Orcas.Networking;

namespace Orcas.Game.Common
{
    /// <summary>
    /// 登录类型
    /// </summary>
    public class LoginType
    {
        public LoginType()
        {
        }
        public LoginType(uint v)
        {
            Value = v;
        }
        public LoginType(int v)
        {
            Value = (uint)v;
        }
        /// <summary>
        /// 登录类型的值
        /// </summary>
        public uint Value { get; set; }
        public static readonly LoginType Guest = new LoginType() { Value = 1 };
        public static readonly LoginType FaceBook = new LoginType() { Value = 2 };
        public static readonly LoginType GameCenter = new LoginType() { Value = 4 };
        public static readonly LoginType Email = new LoginType() { Value = 8 };

        public static LoginType operator |(LoginType num1, LoginType num2)
        {
            return new LoginType() { Value = num1.Value | num2.Value };
        }

        public static LoginType operator &(LoginType num1, LoginType num2)
        {
            return new LoginType() { Value = num1.Value & num2.Value };
        }

        public static bool operator >(LoginType num1, LoginType num2)
        {
            return num1.Value > num2.Value;
        }

        public static bool operator <(LoginType num1, LoginType num2)
        {
            return num1.Value < num2.Value;
        }

        public static bool operator >(LoginType num1, int num2)
        {
            return num1.Value > num2;
        }

        public static bool operator <(LoginType num1, int num2)
        {
            return num1.Value < num2;
        }

        public static bool operator >(LoginType num1, uint num2)
        {
            return num1.Value > num2;
        }

        public static bool operator <(LoginType num1, uint num2)
        {
            return num1.Value < num2;
        }
    }

    [UnityEngine.Scripting.Preserve]
    public class ReqLogin : IReqProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string DeviceCode { get; set; }
        /// <summary>
        /// 设备Id保存值
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string DeviceCodeSaved { get; set; }
        /// <summary>
        /// 广告追踪id
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string IDFA { get; set; }
        /// <summary>
        /// 包名
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string PackageName { get; set; }
        /// <summary>
        /// 玩家来源
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string MediaSource { get; set; }
        /// <summary>
        /// FB token
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string FBToken { get; set; }
        /// <summary>
        /// Firebase token
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string FCMToken { get; set; }
        /// <summary>
        /// 国家码
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string CountryCode { get; set; }
        /// <summary>
        /// 语言码
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string LanguageCode { get; set; }
        /// <summary>
        /// utc 时差
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public byte TimeDiff { get; set; }
        /// <summary>
        /// 登录类型 ios、android、pc
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public byte OsType { get; set; }
        /// <summary>
        /// game token
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string GameToken { get; set; }
        /// <summary>
        /// 客户端版本
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string VersionCode { get; set; }
    }
}