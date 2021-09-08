using Orcas.Ecs.Fsm;
using Orcas.Ecs.Fsm.Interface;
using Orcas.Game.Multiplayer.Proto;
using Orcas.Networking;
using Orcas.Networking.Tcp;
using Unity.Collections;

namespace Orcas.Game.Multiplayer
{
    public interface IRoom : IClientEventHandler
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        long Id { get; }
        /// <summary>
        /// 房间的状态机
        /// </summary>
        IFsm RoomFsm { get; }

        MultiPlayerManager MultiPlayer { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="multiPlayer"></param>
        void SetMultiplayer(MultiPlayerManager multiPlayer);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(IReqProto message);
        /// <summary>
        /// 离开游戏
        /// </summary>
        void SendQuitGame();
        /// <summary>
        /// 关闭房间
        /// </summary>
        void CloseRoom();
        /// <summary>
        /// 更新心跳等
        /// </summary>
        void OnUpdate();
        /// <summary>
        /// 加入游戏
        /// </summary>
        void EnterGame(RltMatchInfo matchResult);
        /// <summary>
        /// 请求开始游戏
        /// </summary>
        void SendStartGame();
        /// <summary>
        /// 获取玩家实体
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        IFsm GetPlayerFsm(int playerId);
        /// <summary>
        /// 通过team的下标获取到队伍的对象
        /// </summary>
        /// <param name="index"></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        NativeArray<ServerObjectComponent> GetTeam(int index, Allocator allocator);
        /// <summary>
        /// 设置玩家状态
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        void SetPlayerState<T>(int playerId, T t) where T : struct, IFsmState;
        /// <summary>
        /// 获取玩家状态
        /// </summary>
        /// <param name="playerId"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetPlayerState<T>(int playerId) where T : struct, IFsmState;
        
        //----------------事件------------------
        /// <summary>
        /// 有玩家进入了房间(包括自己)
        /// </summary>
        void OnPlayerJoined(int playerId);
        /// <summary>
        /// 有玩家退出了房间(包括自己)
        /// </summary>
        void OnPlayerQuitGame(int playerId);
        /// <summary>
        /// 游戏开始
        /// </summary>
        void OnStartGame();
        /// <summary>
        /// 回合开始的时候触发
        /// </summary>
        void OnRoundStart(RltRoundStart proto);
        /// <summary>
        /// 游戏结束的时候触发
        /// </summary>
        void OnGameOver(GameStateReason reason);
        /// <summary>
        /// 重连成功后触发
        /// </summary>
        void OnReconnected(RltReconnect proto);
    }
}