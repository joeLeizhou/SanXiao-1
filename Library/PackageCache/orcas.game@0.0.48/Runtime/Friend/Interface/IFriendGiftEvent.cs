using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Game.Friend
{
    public interface IFriendGiftEvent 
    {
        /// <summary>
        /// 好友列表服务器数据刷新
        /// </summary>
        void OnRefreshFriendServerData();
        
        /// <summary>
        /// 邀请好友失败
        /// </summary>
        /// <param name="code"></param>
        void OnInviteFriendFailed(int code);
        
        /// <summary>
        /// 领取好友礼物失败，如已达到上限
        /// </summary>
        /// <param name="code"></param>
        void OnReceiveGiftFailed(int code);


        /// <summary>
        /// 领取好友礼物成功
        /// </summary>
        /// <param name="totalGiftCount">本次领取总礼物数量</param>
        void OnReceiveGiftSuccess(int totalGiftCount);

        /// <summary>
        /// 领取邀请安装的好友礼物失败
        /// </summary>
        /// <param name="code"></param>
        void OnReceiveInviteRewardFailed(int code);

        /// <summary>
        /// 发送好友礼物失败， 如已达上限
        /// </summary>
        /// <param name="code"></param>
        void OnSendGiftFailed(int code);       
    }
}
