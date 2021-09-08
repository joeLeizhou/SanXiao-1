using Orcas.Networking;

namespace Orcas.Game.Common
{
    [UnityEngine.Scripting.Preserve]
    public class RltSwitchAccount : IRltProto
    {
        [UnityEngine.Scripting.Preserve]
        public ushort ID { get; set; }
        /// <summary>
        /// 用于表示登录是否成功，以及失败的原因
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public byte Code { get; set; }
        /// <summary>
        /// 玩家的游戏ID
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public string Id { get; set; }
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
        /// 账号绑定类型
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public LoginType LoginType { get; set; }
        

        public void Deal()
        {
        }
    }
}