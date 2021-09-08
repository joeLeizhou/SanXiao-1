using System.Collections;
using System.Collections.Generic;
using Orcas.Core;
using Orcas.Game.Common;
using UnityEngine;
using Orcas.Networking;
using UnityEngine.PlayerLoop;

namespace Orcas.Game.Friend
{

    public enum FriendType
    {
        Facebook = 1
    }
    
    public class FriendManager : IManager
    {
        
        /// <summary>
        /// 网络客户端 
        /// </summary>
        public IClient Client;
        
        /// <summary>
        /// 社交账号类型
        /// </summary>
        public FriendType Type;


        public IFriendGiftEvent Listener;
        
        /// <summary>
        /// 服务器数据好友数据列表
        /// </summary>
        public FriendServerInfo[] FriendServerDataList;


        /// <summary>
        /// 好友送礼物限制数据
        /// </summary>
        public FriendGiftLimitData GiftLimitData;


        /// <summary>
        /// 好友礼物配置数据
        /// </summary>
        public FriendGiftConfigData GiftConfigData;
        
        
        /// <summary>
        /// 请求好友列表
        /// </summary>
        public void RequestFriendServerData(FriendType type, string[] socialAccountIds)
        {
            if (socialAccountIds == null || socialAccountIds.Length <= 0) return;
            ReqFriendList proto = new ReqFriendList();
            proto.ID = CommonProtoId.ReqFriendList;
            proto.Type = (byte)type;
            proto.SocialAccountIds = socialAccountIds;
            Client?.SendMessage(proto);
        }

        /// <summary>
        /// 客户端请求发送好友礼物
        /// </summary>
        /// <param name="userIdArr"></param>
        public void SendGifts(int[] userIdArr)
        {
            ReqSendFriendGift proto = new ReqSendFriendGift();
            proto.ID = CommonProtoId.ReqSendFriendGift;
            proto.SendIdList = userIdArr;
            Client?.SendMessage(proto);
        }

        /// <summary>
        /// 发送所有好友礼物
        /// </summary>
        public void SendAllGifts(int curTime)
        {
            if (GiftLimitData.NeedSendLimit && GiftLimitData.CurrentSend >= GiftLimitData.SendLimit)
            {
                return;
            }
            
            List<int> sendList = new List<int>();
            for (int i = 0; i < FriendServerDataList.Length; i++)
            {
                var data = FriendServerDataList[i];
                bool canSend = CheckCanSendFriendGift(data, curTime);
                if (canSend)
                {
                    sendList.Add(data.ServerUserId);
                }
            }

            if (sendList.Count > 0)
            {
                SendGifts(sendList.ToArray());
            }
        }

        /// <summary>
        /// 客户端请求接受好友礼物
        /// </summary>
        /// <param name="userIdArr"></param>
        public void ReceiveGifts(int[] userIdArr)
        {
            ReqReceiveFriendGift proto = new ReqReceiveFriendGift();
            proto.ID = CommonProtoId.ReqReceiveFriendGift;
            proto.ReceiveIdList = userIdArr;
            Client?.SendMessage(proto);
        }

        /// <summary>
        /// 接收所有好友礼物
        /// </summary>
        public void ReceiveAllGifts(int curTime)
        {
            List<int> receiveList = new List<int>();
            List<int> inviteList = new List<int>();
            for (int i = 0; i < FriendServerDataList.Length; i++)
            {
                var data = FriendServerDataList[i];
                bool canReceive = CheckCanReceiveFriendGift(data, curTime);
                if (canReceive)
                {
                    if (data.InviteState == 1)
                    {
                        inviteList.Add(data.ServerUserId); 
                    }
                    else
                    {
                        receiveList.Add(data.ServerUserId);
                    }
                }
            }

            if (inviteList.Count > 0)
            {
                ReceiveInviteGifts(inviteList.ToArray());
            }

            if (receiveList.Count > 0)
            {
                ReceiveGifts(receiveList.ToArray());
            }
        }

        /// <summary>
        /// 客户端请求接受邀请好友礼物
        /// </summary>
        /// <param name="userIdArr"></param>
        public void ReceiveInviteGifts(int[] userIdArr)
        {
            for (int i = 0; i < userIdArr.Length; i++)
            {
                ReqReceiveInviteFriendGift proto = new ReqReceiveInviteFriendGift();
                proto.ID = CommonProtoId.ReqReceiveInviteFriendGift;
                proto.UserId = userIdArr[i];
                Client?.SendMessage(proto);
            }
        }


        public void SendInviteFriend(FriendType type, string[] socialAccountId)
        {
            ReqInviteFriend proto = new ReqInviteFriend();
            proto.ID = CommonProtoId.ReqInviteFriend;
            proto.Type = (byte) (type);
            proto.SocialAccountIdList = socialAccountId;
            Client.SendMessage(proto);
        }

        /// <summary>
        /// 检查能否给该好友送金币
        /// </summary>
        /// <param name="data">好友信息</param>
        /// <param name="curTime">服务器时间</param>
        /// <returns></returns>
        public bool CheckCanSendFriendGift(FriendServerInfo data, int curTime)
        {
            bool canSend = data.LastSendTime + GiftLimitData.SendCdTime < curTime;
            if (GiftLimitData.NeedSendLimit && GiftLimitData.CurrentSend >= GiftLimitData.SendLimit)
            {
                canSend = false;
            }
            
            return canSend;
        }
        
        /// <summary>
        /// 检查能否给该好友送金币
        /// </summary>
        /// <param name="data">好友信息</param>
        /// <param name="curTime">服务器时间</param>
        /// <returns></returns>
        public bool CheckCanSendFriendGiftByUid(int userId, int curTime)
        {
            if (FriendServerDataList != null)
            {
                foreach (var info in FriendServerDataList)
                {
                    if (info.ServerUserId == userId)
                    {
                        return CheckCanSendFriendGift(info, curTime);
                    }      
                }
            }
            return false;
        }
        
        /// <summary>
        /// 检查能否给该好友送金币
        /// </summary>
        /// <param name="data">好友信息</param>
        /// <param name="curTime">服务器时间</param>
        /// <returns></returns>
        public bool CheckCanSendFriendGiftBySocialAccountId(string sId, int curTime)
        {
            if (FriendServerDataList != null)
            {
                foreach (var info in FriendServerDataList)
                {
                    if (info.SocialAccountId == sId)
                    {
                        return CheckCanSendFriendGift(info, curTime);
                    }      
                }
            }
            return false;
        }


        /// <summary>
        /// 检查能否接收好友礼物
        /// </summary>
        /// <param name="data">好友数据</param>
        /// <param name="curTime">服务器时间</param>
        /// <returns></returns>
        public bool CheckCanReceiveFriendGift(FriendServerInfo data, int curTime)
        {
            bool canReceive = false;
            if (data.ReceivedCount > 0)
            {
                canReceive = data.LastReceiveTime + GiftLimitData.ReceiveCdTime < curTime;
            }

            if (GiftLimitData.NeedReceiveLimit && GiftLimitData.CurrentReceive >= GiftLimitData.ReceiveLimit)
            {
                canReceive = false;
            }

            if (data.InviteState == 1)
            {
                canReceive = true;
            }

            return canReceive;
        }
        
        /// <summary>
        /// 检查能否给该好友送金币
        /// </summary>
        /// <param name="data">好友信息</param>
        /// <param name="curTime">服务器时间</param>
        /// <returns></returns>
        public bool CheckCanReceiveFriendGiftByUid(int userId, int curTime)
        {
            if (FriendServerDataList != null)
            {
                foreach (var info in FriendServerDataList)
                {
                    if (info.ServerUserId == userId)
                    {
                        return CheckCanReceiveFriendGift(info, curTime);
                    }      
                }
            }
            return false;
        }
        
        
        /// <summary>
        /// 检查能否给该好友送金币
        /// </summary>
        /// <param name="data">好友信息</param>
        /// <param name="curTime">服务器时间</param>
        /// <returns></returns>
        public bool CheckCanReceiveFriendGiftBySocialAccountId(string sId, int curTime)
        {
            if (FriendServerDataList != null)
            {
                foreach (var info in FriendServerDataList)
                {
                    if (info.SocialAccountId == sId)
                    {
                        return CheckCanReceiveFriendGift(info, curTime);
                    }      
                }
            }
            return false;
        }

        public void Init<T>(IClient client) where T : IFriendGiftEvent, new()
        {
            Client = client;
            Listener = new T();
            GiftLimitData = new FriendGiftLimitData();
            GiftConfigData = new FriendGiftConfigData();
            ProtocolFactory.Instance.AddProto<ReqFriendList>(CommonProtoId.ReqFriendList);
            ProtocolFactory.Instance.AddProto<RltFriendList>(CommonProtoId.RltFriendList);
            ProtocolFactory.Instance.AddProto<ReqInviteFriend>(CommonProtoId.ReqInviteFriend);
            ProtocolFactory.Instance.AddProto<RltInviteFriend>(CommonProtoId.RltInviteFriend);
            ProtocolFactory.Instance.AddProto<ReqReceiveFriendGift>(CommonProtoId.ReqReceiveFriendGift);
            ProtocolFactory.Instance.AddProto<RltReceiveFriendGift>(CommonProtoId.RltReceiveFriendGift);
            ProtocolFactory.Instance.AddProto<ReqSendFriendGift>(CommonProtoId.ReqSendFriendGift);
            ProtocolFactory.Instance.AddProto<RltSendFriendGift>(CommonProtoId.RltSendFriendGift);
            ProtocolFactory.Instance.AddProto<ReqReceiveInviteFriendGift>(CommonProtoId.ReqReceiveInviteFriendGift);
            ProtocolFactory.Instance.AddProto<RltReceiveInviteFriendGift>(CommonProtoId.RltReceiveInviteFriendGift);
            ProtocolFactory.Instance.AddProto<RltFriendStatus>(CommonProtoId.RltFriendStatus);
        }

        public void Init()
        {
            GiftLimitData = new FriendGiftLimitData();
            GiftConfigData = new FriendGiftConfigData();
            ProtocolFactory.Instance.AddProto<ReqFriendList>(CommonProtoId.ReqFriendList);
            ProtocolFactory.Instance.AddProto<RltFriendList>(CommonProtoId.RltFriendList);
            ProtocolFactory.Instance.AddProto<ReqInviteFriend>(CommonProtoId.ReqInviteFriend);
            ProtocolFactory.Instance.AddProto<RltInviteFriend>(CommonProtoId.RltInviteFriend);
            ProtocolFactory.Instance.AddProto<ReqReceiveFriendGift>(CommonProtoId.ReqReceiveFriendGift);
            ProtocolFactory.Instance.AddProto<RltReceiveFriendGift>(CommonProtoId.RltReceiveFriendGift);
            ProtocolFactory.Instance.AddProto<ReqSendFriendGift>(CommonProtoId.ReqSendFriendGift);
            ProtocolFactory.Instance.AddProto<RltSendFriendGift>(CommonProtoId.RltSendFriendGift);
            ProtocolFactory.Instance.AddProto<ReqReceiveInviteFriendGift>(CommonProtoId.ReqReceiveInviteFriendGift);
            ProtocolFactory.Instance.AddProto<RltReceiveInviteFriendGift>(CommonProtoId.RltReceiveInviteFriendGift);
            ProtocolFactory.Instance.AddProto<RltFriendStatus>(CommonProtoId.RltFriendStatus);
        }

        private void UploadOnlineStatus(bool foucus)
        {
            ReqUploadStatus proto = new ReqUploadStatus();
            proto.ID = CommonProtoId.ReqUploadStatus;
            proto.Status = foucus ? (byte)4 : (byte)3;
            Client?.SendMessage(proto);
        }

        public void Update(uint currentFrameCount)
        {
            
        }

        public void OnPause()
        {
            UploadOnlineStatus(false);
        }

        public void OnResume()
        {
            UploadOnlineStatus(true);
        }

        public void OnDestroy()
        {
            
        }
    }
}


