using Orcas.Networking;

namespace Orcas.Game.Common
{
    public static class LoginResultCode
    {
        public const byte Success = 0;
        public const byte UnknownFail = 1;
    }

    [UnityEngine.Scripting.Preserve]
    public class RltLogin : IRltProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
        /// <summary>
        /// 用于表示登录是否成功，以及失败的原因
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public short ResCode { get; set; }
        /// <summary>
        /// 玩家的游戏ID
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public int UserId { get; set; }
        /// <summary>
        /// 游戏token
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string Token { get; set; }
        /// <summary>
        /// 服务器时间
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public long ServerTime { get; set; }
        /// <summary>
        /// 账号绑定
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public LoginType Binder { get; set; }
        /// <summary>
        /// 登录设备id
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string DeviceCode { get; set; }
        /// <summary>
        /// 登录玩家姓名
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string UserName { get; set; }
        public void Deal()
        {
            var str = $"code:{ResCode},uid:{UserId},binder:{Binder},serverTime:{ServerTime},token:{Token},deviceCode:{DeviceCode}";
            UnityEngine.Debug.Log("rlt Res:" + str);
        }
    }
}