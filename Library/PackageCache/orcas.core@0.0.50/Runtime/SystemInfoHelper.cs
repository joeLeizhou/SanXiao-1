using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.Core.NativeUtils;
using System;
#if UNITY_IPHONE
using UnityEngine.iOS;
#endif

namespace Orcas.Core.Tools
{
    public static partial class SystemInfoHelper
    {
#if TEST
        public static bool Test = true;
#else
        public static bool Test = false;
#endif
        public const int PC = 0;
        public const int iOS = 1;
        public const int Android = 2;

        public static string GetDeviceCode()
        {
            return NativeUtils.NativeUtils.GetDeviceCode();
        }

        public static string GetDeviceCodeSaved()
        {
            return GetDeviceCode();
        }

        public static byte GetTimeDiff()
        {
            return (byte)System.TimeZoneInfo.Local.BaseUtcOffset.Hours;
        }

        public static string GetVersionCode()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.HotfixVersion, Application.version);
        }

        public static string GetGameToken()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.GameToken, "");
        }

        public static string GetApplicationVersion()
        {
            return Application.version;
        }

        //获取设备是安卓还是ios还是pc
        public static int GetPlatform()
        {
            if (Application.isEditor)
                return PC;
            else if (Application.platform == RuntimePlatform.Android)
                return Android;
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return iOS;
            return PC;
        }

        public static string GetCountryCode()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.CountryCode, "us");
        }

        public static string GetLanguageCode()
        {
            return NativeUtils.NativeUtils.GetDefaultLanguage();
        }

        public static string GetPackageName()
        {
            return Application.identifier;
        }

        public static string GetOperatingSystem()
        {
            return SystemInfo.operatingSystem;
        }

        public static string GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }

        public static int GetDeviceBattery()
        {
            return SystemInfo.batteryLevel >= 0.0f ? (int)(SystemInfo.batteryLevel * 100.0f) : -1;
        }

        public static bool IsInWifi()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }

        public static bool IsNetReachable()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        public static string GetScreenSize()
        {
            return Screen.currentResolution.ToString();
        }

        public static int GetMemory()
        {
            return SystemInfo.systemMemorySize;
        }

        public static int GetCpuCount()
        {
            return SystemInfo.processorCount;
        }

        public static int GetCpuFrequency()
        {
            return SystemInfo.processorFrequency;
        }

        public static string GetCpuName()
        {
            return SystemInfo.processorType;
        }

        public static string GetIOSVersion()
        {
#if UNITY_IPHONE
		return Device.systemVersion;
#else
            return "";
#endif
        }

        public static string GetMediaSource()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.MediaSource, "");
        }
        public static string GetFBToken()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.FBToken, "");
        }
        public static string GetFCMToken()
        {
            return PlayerPrefsManager.GetString(PlayerPrefHelper.FCMToken, "");
        }
        public static int CompareiOSVersion(string iosVersion, int dValue)
        {
            // #if UNITY_IOS && !UNITY_EDITOR
            var osVersion = SystemInfo.operatingSystem;
            try
            {
                var opss = osVersion.Split(' ');
                return new Version(opss[opss.Length - 1]).CompareTo(new Version(iosVersion));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            // #endif
            return dValue;
        }
        public static int CompareiOSModel(string iPhoneModeVersion, string iPadModeVersion, int dValue)
        {
            // #if UNITY_IOS && !UNITY_EDITOR
            var model = SystemInfo.deviceModel;
            try
            {
                if (model.StartsWith("iPhone", StringComparison.Ordinal))
                    return new Version(model.Remove(0, 6), ',').CompareTo(new Version(iPhoneModeVersion, ','));
                else if (model.StartsWith("iPad", StringComparison.Ordinal))
                    return new Version(model.Remove(0, 4), ',').CompareTo(new Version(iPadModeVersion, ','));
                else
                    return dValue;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            // #endif
            return dValue;
        }
    }

}

