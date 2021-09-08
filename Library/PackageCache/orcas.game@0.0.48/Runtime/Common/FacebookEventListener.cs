// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Orcas.FBSdk;
// using Orcas.Game.Friend;

// namespace Orcas.Game.Common
// {
//     public class FacebookEventListener : IFacebookEvent
//     {
//         public void OnGetUserName(string fbName, string fbToken)
//         {
            
//         }

//         public void OnBindFacebook(string fbToken)
//         {
            
//         }

//         public void OnBindFacebookFailed()
//         {
            
//         }

//         public void OnGetPlayerInfo()
//         {
            
//         }

//         public void OnGetFriendInfo()
//         {
//             var giftMgr = GameManager.Instance.GetManager<FriendManager>();
//             giftMgr?.RequestFriendServerData(FriendType.Facebook, FacebookManager.FriendsIDList);
//         }

//         public void OnSendInviteFriend(string[] friendList)
//         {
//             var giftMgr = GameManager.Instance.GetManager<FriendManager>();
//             giftMgr?.SendInviteFriend(FriendType.Facebook, friendList);
//         }

//         public void OnRequestAppUsers(string userId)
//         {
            
//         }
//     }
// }
