using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer
{
    /// <summary>
    /// 匹配事件，用于记录匹配结果
    /// </summary>
    public enum MatchEvent
    {
        Success = 0,
        ClientOverTime = 1,
        ClientCanceled = 2,
        Matching = 3,
        NoMatch = 4,
    }

    /// <summary>
    /// 匹配状态
    /// </summary>
    public enum MatchState
    {
        None = 0,
        Matching = 1,
        Canceling = 2,
        Over = 3
    }
    public interface IMatcher
    {
        /// <summary>
        /// 计时器，从匹配开始时计时
        /// </summary>
        Timer Timer { get; }
        /// <summary>
        /// 是否正在匹配
        /// </summary>
        MatchState State { get; }
        /// <summary>
        /// 设置多人游戏管理器
        /// </summary>
        /// <param name="multiPlayer"></param>
        void SetMultiplayer(MultiPlayerManager multiPlayer);
        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stage"></param>
        void Match(int type, int stage);
        /// <summary>
        /// 取消匹配
        /// </summary>
        void Cancel();
        /// <summary>
        /// 重置，准备重新匹配
        /// </summary>
        void Reset();
        /// <summary>
        /// 匹配结果，只有状态
        /// </summary>
        void OnMatch(RltMatch serverResult);
        /// <summary>
        /// 匹配成功的数据
        /// </summary>
        /// <param name="serverResult"></param>
        void OnMatchInfo(RltMatchInfo serverResult);
        /// <summary>
        /// 取消匹配返回
        /// </summary>
        /// <param name="serverResult"></param>
        void OnMatchCancel(RltCancelMatch serverResult);
    }
}