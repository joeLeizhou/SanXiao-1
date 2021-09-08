using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class RltSendFriendGift : IRltProto
    {
        public ushort ID { get; set; }

        public short Code;

        public int TotalSendTimes;

        public FriendGiftSendInfo[] SendList;         
        
        public void Deal()
        {
            var giftMgr = GameManager.Instance.GetManager<FriendManager>();
            if (Code == 1)
            {
                if (giftMgr != null)
                {

                    giftMgr.GiftLimitData.CurrentSend = TotalSendTimes;

                    if (giftMgr.FriendServerDataList != null)
                    {
                        for (int i = 0; i < SendList.Length; i++)
                        {
                            var sendData = SendList[i];
                            for (int j = 0; j < giftMgr.FriendServerDataList.Length; j++)
                            {
                                var friendData = giftMgr.FriendServerDataList[j];
                                if (friendData.ServerUserId == sendData.UserId)
                                {
                                    friendData.LastSendTime = sendData.LastSendTime;
                                }
                            }
                        }
                    }
                    
                    giftMgr.Listener?.OnRefreshFriendServerData();
                }
            }
            else
            {
                giftMgr?.Listener?.OnSendGiftFailed(Code);
            }
        }
    }
}
