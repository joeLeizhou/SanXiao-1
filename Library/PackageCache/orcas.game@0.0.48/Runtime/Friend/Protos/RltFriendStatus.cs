using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class RltFriendStatus : IRltProto
    {
        public ushort ID { get; set; }
        public byte Status;
        public int UserId;
        
        public void Deal()
        {
            var giftMgr = GameManager.Instance.GetManager<FriendManager>();
            if (giftMgr != null && giftMgr.FriendServerDataList != null)
            {
                for (int i = 0; i < giftMgr.FriendServerDataList.Length; i++)
                {
                    var data = giftMgr.FriendServerDataList[i];
                    if (data.ServerUserId == UserId)
                    {
                        data.Status = Status;
                    }
                }
                giftMgr.Listener?.OnRefreshFriendServerData();
            }
        }
    }
}
