using System;
using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using UnityEngine;
using Orcas.Networking;

namespace Orcas.Game.Friend
{
    [UnityEngine.Scripting.Preserve]
    public class RltFriendList : IRltProto
    {
        public ushort ID { get; set; }
        
        public byte FriendType;
    
        public int RefreshTimeStamp;

        public bool NeedReceiveLimit;
    
        public int GiftReceiveLimit;

        public bool NeedSendLimit;

        public int GiftSendLimit;

        public int CurSend;

        public int CurReceive;

        public int SendCdTime;

        public int ReceiveCdTime;

        public int GiftType;

        public int GiftNum;

        public FriendServerInfo[] FriendList;

        public void Deal()
        {
            var giftMgr = GameManager.Instance.GetManager<FriendManager>();
            if (giftMgr != null)
            {
                giftMgr.GiftConfigData.GiftType = GiftType;
                giftMgr.GiftConfigData.GiftNum = GiftNum;

                giftMgr.GiftLimitData.SendLimit = GiftSendLimit;
                giftMgr.GiftLimitData.NeedSendLimit = NeedSendLimit;
                giftMgr.GiftLimitData.ReceiveLimit = GiftReceiveLimit;
                giftMgr.GiftLimitData.NeedReceiveLimit = NeedReceiveLimit;
                giftMgr.GiftLimitData.RefreshTime = RefreshTimeStamp;
                giftMgr.GiftLimitData.CurrentSend = CurSend;
                giftMgr.GiftLimitData.CurrentReceive = CurReceive;
                giftMgr.GiftLimitData.SendCdTime = SendCdTime;
                giftMgr.GiftLimitData.ReceiveCdTime = ReceiveCdTime;

                giftMgr.Type = (FriendType)FriendType;
                
                giftMgr.FriendServerDataList = new FriendServerInfo[FriendList.Length];
                Array.Copy(FriendList, giftMgr.FriendServerDataList, FriendList.Length);
                
                giftMgr.Listener?.OnRefreshFriendServerData();
            }
        }
    }
}
