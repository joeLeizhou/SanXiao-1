using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class RltInviteFriend : IRltProto
    {
        public ushort ID { get; set; }
        public short Code;
        
        public void Deal()
        {
            if (Code != 1)
            {
                var giftMgr = GameManager.Instance.GetManager<FriendManager>();
                giftMgr?.Listener?.OnInviteFriendFailed(Code);
            }
        }
    }
}
