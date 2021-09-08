using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class ReqInviteFriend : IReqProto
    {
        public ushort ID { get; set; }
        public byte Type;
        public string[] SocialAccountIdList;
    }
}
