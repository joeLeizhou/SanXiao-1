﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class ReqReceiveFriendGift : IReqProto
    {
        public ushort ID { get; set; }
        public int[] ReceiveIdList;
    }
}
