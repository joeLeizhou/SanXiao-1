using System.Collections;
using System.Collections.Generic;
using Orcas.Networking;
using UnityEngine;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class ReqFriendList : IReqProto
    {
        public ushort ID { get; set; }

        public byte Type;

        public string[] SocialAccountIds;
    }    
}


