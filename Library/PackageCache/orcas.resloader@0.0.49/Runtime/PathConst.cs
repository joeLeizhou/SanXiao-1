using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Resources
{
    public static class PathConst
    {
        public static readonly string FileListName = "FileList.csv";
        public static readonly string FileListShortName = "FileList";
        public static readonly string AllFileListName = "AllFileList.csv";
        public static readonly string CopyFileListName = "CopyFileList.csv";
        public static readonly string PacksFileListName = "PackFileList.csv";
        public static readonly string PacksManifestName = "PackManifest";
        public static readonly string AssetBundlePath = "AssetBundle/";
        public static readonly string ManifestName = "Manifest";

        public static string[] ServerUrlArr = new string[] { "" };
        private static int _serverIndex = 0;

#if UNITY_ANDROID
        public static readonly string OSDir = "Android";
#elif UNITY_IOS
        public static readonly string OSDir = "iOS";
#elif UNITY_EDITOR || UNITY_STANDALONE
        public static readonly string OSDir = "StandAlone";
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        public static readonly string SourceOSUrl = $"{Application.streamingAssetsPath}/{OSDir}/";
#else
        public static readonly string SourceOSUrl = $"file:///{Application.streamingAssetsPath}/{OSDir}/";
#endif

        public static readonly string SourceOSPath = $"{Application.streamingAssetsPath}/{OSDir}/";

        public static readonly string DestPath = Application.persistentDataPath + "/";
        public static readonly string DestUrl = $"file:///{Application.persistentDataPath}/";

#if !UNITY_EDITOR
        public static readonly HotfixVersion AppVer = new HotfixVersion(Application.version);
#else
        public static HotfixVersion AppVer = new HotfixVersion(Application.version);
#endif
        public static string GetServerRootUrl(int offset = 0)
        {
            _serverIndex += offset;
            _serverIndex %= ServerUrlArr.Length;
            return ServerUrlArr[_serverIndex];
        }
        public static void SetServerRootUrls(string[] urls)
        {
            for (int i = 0; i < urls.Length; i++)
            {
                urls[i] = CheckAndFixPathRoot(urls[i]);
            }
            ServerUrlArr = urls;
        }

        public static string CheckAndFixPathRoot(string path)
        {
            if (path == null || path.Length == 0)
                return path;
            if (path[path.Length - 1] != '/' && path[path.Length - 1] != '\\')
                return path + "/";
            return path;
        }

        public static string GetPackOSPath(string root)
        {
            return $"{root}Pack/{OSDir}/";
        }
        public static string GetHotVersionPath(string root, HotfixVersion buildV)
        {
            return $"{root}Hotfix/{OSDir}/LV{buildV.ToStringShort()}/Version.csv";
        }
        public static string GetHotfixOSBuildPath(string root, HotfixVersion buildV)
        {
            return $"{root}Hotfix/{OSDir}/LV{buildV.ToStringShort()}/";
        }
        public static string GetHotfixOSVersionPath(string root, HotfixVersion hotV, HotfixVersion buildV)
        {
            return $"{root}Hotfix/{OSDir}/LV{buildV.ToStringShort()}/{hotV}/";
        }

        public static string GetHotfixOSBundlePath(string root, HotfixVersion buildV)
        {
            return $"{root}Hotfix/{OSDir}/LV{buildV.ToStringShort()}/{AssetBundlePath}";
        }

        public static string GetPackFileList(int index)
        {
            return $"FileList{index}.csv";
        }

        public static string GetCustomPackManifest(int index)
        {
            return index == 0 ? "PackManifest" : $"PackManifest{index}";
        }

    }
}