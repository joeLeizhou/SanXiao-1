using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    
    [UnityEngine.Scripting.Preserve]
    public class RltReceiveFriendGift : IRltProto
    {
        public ushort ID { get; set; }

        public short Code;

        public int TotalReceiveTime;

        public int TotalGiftNum;

        public FriendGiftReceiveInfo[] ReceiveList;

        
        public void Deal()
        {
            var giftMgr = GameManager.Instance.GetManager<FriendManager>();
            if (Code == 1)
            {
                if (giftMgr != null)
                {

                    giftMgr.GiftLimitData.CurrentReceive = TotalReceiveTime;

                    if (giftMgr.FriendServerDataList != null)
                    {
                        for (int i = 0; i < ReceiveList.Length; i++)
                        {
                            var receiveData = ReceiveList[i];
                            for (int j = 0; j < giftMgr.FriendServerDataList.Length; j++)
                            {
                                var friendData = giftMgr.FriendServerDataList[j];
                                if (friendData.ServerUserId == receiveData.UserId)
                                {
                                    friendData.ReceivedCount = receiveData.ReceivedCount;
                                    friendData.LastReceiveTime = receiveData.LastReceiveTime;
                                }
                            }
                        }
                    }
                    
                    giftMgr.Listener?.OnRefreshFriendServerData();
                    giftMgr.Listener?.OnReceiveGiftSuccess(TotalGiftNum);
                }
            }
            else
            {
                giftMgr?.Listener?.OnReceiveGiftFailed(Code);
            }
        }
    }
}
