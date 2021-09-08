using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class RltReceiveInviteFriendGift : IRltProto
    {
        public ushort ID { get; set; }

        public short Code;

        public int UserId; 
        
        public void Deal()
        {
            var giftMgr = GameManager.Instance.GetManager<FriendManager>();
            // Code：1代表领取成功， 2220代表已经领取过了
            if (Code == 1 || Code == 2220)
            {
                SetInviteStateReceived(giftMgr, UserId);
                giftMgr?.Listener?.OnRefreshFriendServerData();
            }

            if (Code != 1)
            {
                giftMgr?.Listener?.OnReceiveInviteRewardFailed(Code);
            }
        }


        private void SetInviteStateReceived(FriendManager giftMgr, int userId)
        {
            if (giftMgr != null && giftMgr.FriendServerDataList != null)
            {
                for (int i = 0; i < giftMgr.FriendServerDataList.Length; i++)
                {
                    var friendData = giftMgr.FriendServerDataList[i];
                    if (friendData.ServerUserId == userId)
                    {
                        friendData.InviteState = 2;
                    }
                }
            }
        }
    }
}
