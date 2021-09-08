using System;
using Orcas.Networking;
using UnityEngine;
using Orcas.Core.NativeUtils;
using Orcas.Core.Tools;

namespace Orcas.Game.Common
{
    public class LoginBridgeBase
    {
        protected string GetCountryCode()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.CountryCode, "");
        }
        protected string GetGameToken()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.GameToken, "");
        }

        protected string GetVersionCode()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.HotfixVersion, Application.version);
        }
        internal void OnLogin(RltLogin proto)
        {
            if (proto.ResCode == 1)
            {
                PlayerPrefsManager.SetInt(PlayerPrefHelper.UserIDInt, proto.UserId);
                PlayerPrefsManager.SetString(PlayerPrefHelper.UserID, proto.UserId.ToString());
                PlayerPrefsManager.SetString(PlayerPrefHelper.UserName, proto.UserName);
                PlayerPrefsManager.SetString(PlayerPrefHelper.GameToken, proto.Token);
                PlayerPrefsManager.SetInt(PlayerPrefHelper.USER_BINDER, (int)proto.Binder.Value);
                if (Application.platform == RuntimePlatform.IPhonePlayer && string.IsNullOrEmpty(proto.DeviceCode) == false)
                {
                    PlayerPrefsManager.SetString(PlayerPrefHelper.DevideCode, proto.DeviceCode);
                    NativeUtils.SaveDeviceCode(proto.DeviceCode);
                }
                PlayerPrefsManager.Save();
            }
        }

        public ReqLogin GetReqLoginProto(LoginType loginType, string accountId)
        {
            return new ReqLogin()
            {
                ID = CommonProtoId.ReqLogin,
                DeviceCode = SystemInfoHelper.GetDeviceCode(),
                DeviceCodeSaved = SystemInfoHelper.GetDeviceCodeSaved(),
                IDFA = NativeUtils.GetDeviceIdfa(),
                PackageName = SystemInfoHelper.GetPackageName(),
                MediaSource = SystemInfoHelper.GetMediaSource(),
                FBToken = SystemInfoHelper.GetFBToken(),
                FCMToken = SystemInfoHelper.GetFCMToken(),
                CountryCode = SystemInfoHelper.GetCountryCode(),
                LanguageCode = NativeUtils.GetDefaultLanguage(),
                TimeDiff = SystemInfoHelper.GetTimeDiff(),
                OsType = (byte)SystemInfoHelper.GetPlatform(),
                GameToken = SystemInfoHelper.GetGameToken(),
                VersionCode = SystemInfoHelper.GetVersionCode()
            };
        }
    }
}