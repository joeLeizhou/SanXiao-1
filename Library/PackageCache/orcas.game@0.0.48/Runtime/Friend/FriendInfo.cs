using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    /// <summary>
    /// 好友服务器数据结构
    /// </summary>
    public class FriendServerInfo
    {
        /// <summary>
        /// 服务器的User Id
        /// </summary>
        public int ServerUserId;
            
        /// <summary>
        /// Facebook Id
        /// </summary>
        public string SocialAccountId;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 用户头像链接
        /// </summary>
        public string AvatarUrl;

        /// <summary>
        /// 在线状态
        /// </summary>
        public byte Status;

        /// <summary>
        /// 上次登出时间
        /// </summary>
        public int LogoutTime;

        /// <summary>
        /// 累积胜场数
        /// </summary>
        public int Win;

        /// <summary>
        /// 累积负场数
        /// </summary>
        public int Lose;

        /// <summary>
        /// 累积还未领取的已经接收到该好友的礼物数
        /// </summary>
        public short ReceivedCount;

        /// <summary>
        /// 上次接收该好友礼物的时间
        /// </summary>
        public int LastReceiveTime;

        /// <summary>
        /// 上次发送给该好友礼物的时间
        /// </summary>
        public int LastSendTime;

        /// <summary>
        /// 邀请安装的状态
        /// </summary>
        public byte InviteState;
    }
    [UnityEngine.Scripting.Preserve]
    /// <summary>
    /// 好友礼物领取限制
    /// </summary>
    public class FriendGiftLimitData
    {
        /// <summary>
        /// 当前发送礼物限制
        /// </summary>
        public int SendLimit;

        /// <summary>
        /// 是否需要发送限制
        /// </summary>
        public bool NeedSendLimit;
        
        /// <summary>
        /// 当前接受礼物限制
        /// </summary>
        public int ReceiveLimit;

        /// <summary>
        /// 是否需要接收限制
        /// </summary>
        public bool NeedReceiveLimit;
        
        /// <summary>
        /// 当前已经发送数量
        /// </summary>
        public int CurrentSend;
        
        /// <summary>
        /// 当前已经接受数量
        /// </summary>
        public int CurrentReceive;
        
        /// <summary>
        /// 限制刷新时间
        /// </summary>
        public int RefreshTime;


        /// <summary>
        /// 发送CD时间
        /// </summary>
        public int SendCdTime;

        /// <summary>
        /// 接收CD时间
        /// </summary>
        public int ReceiveCdTime;
    }

    /// <summary>
    /// 好友礼物配置数据
    /// </summary>
    public class FriendGiftConfigData
    {
        /// <summary>
        /// 礼物类型
        /// </summary>
        public int GiftType;

        
        /// <summary>
        /// 礼物数量
        /// </summary>
        public int GiftNum;
    }


    public class FriendGiftSendInfo
    {
        /// <summary>
        /// 玩家游戏Id
        /// </summary>
        public int UserId;

        /// <summary>
        /// 上次发送该好友礼物的时间
        /// </summary>
        public int LastSendTime;
    }

    public class FriendGiftReceiveInfo
    {
        /// <summary>
        /// 玩家游戏Id
        /// </summary>
        public int UserId;

        /// <summary>
        /// 累积还未领取的已经接收到该好友的礼物数
        /// </summary>
        public short ReceivedCount;
        
        /// <summary>
        /// 上次接收该好友礼物的时间
        /// </summary>
        public int LastReceiveTime;
    }
}

