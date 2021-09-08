using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using Orcas.AssetBuilder;
using Orcas.AssetBuilder.Editor;
using System.IO;

namespace Orcas.Resources.Editor
{
    public static class ProjectBuilder
    {
        internal static bool CheckScriptingDefines(BuildTargetGroup group, string includeDefines, string excudeDefines)
        {
            string definesStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            Debug.Log("defines " + definesStr + " in " + includeDefines + " ex " + excudeDefines);
            var defines = new List<string>(definesStr.Split(';'));
            var inDefs = includeDefines.Split(';');
            var exDefs = excudeDefines.Split(';');

            var addDefs = new List<string>();
            var removeDefs = new List<string>();
            for (int i = 0; i < inDefs.Length; i++)
            {
                if (string.IsNullOrEmpty(inDefs[i]) == false && defines.IndexOf(inDefs[i]) == -1)
                {
                    addDefs.Add(inDefs[i]);
                }
            }
            for (int i = 0; i < exDefs.Length; i++)
            {
                if (string.IsNullOrEmpty(exDefs[i]) == false && defines.IndexOf(exDefs[i]) > -1)
                {
                    removeDefs.Add(exDefs[i]);
                }
            }

            if ((addDefs.Count + removeDefs.Count) == 0)
                return true;
            var message = "";
            if (addDefs.Count > 0)
            {
                message += " 需要添加 " + string.Join(";", addDefs);
            }
            if (removeDefs.Count > 0)
            {
                message += " 需要删除 " + string.Join(";", removeDefs);
            }

            // if (EditorUtility.DisplayDialog("宏不对应", message, "确认设置宏", "取消"))
            {
                for (int i = 0; i < removeDefs.Count; i++)
                {
                    defines.Remove(removeDefs[i]);
                }
                for (int i = 0; i < addDefs.Count; i++)
                {
                    defines.Add(addDefs[i]);
                }
                var defs = string.Join(";", defines);
                Debug.LogWarning("set defs " + defs);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defs);
                return true;
            }
            // return false;
        }

        internal static bool CheckResources(ProjectBuilderSetting setting)
        {
            if (AssetBundleBuilder.CheckIfBuild(setting.MainABSetting) == false)
            {
                Selection.activeObject = setting.MainABSetting;
                return false;
            }
            foreach (var abSetting in setting.AdditionABSetting)
            {
                if (AssetBundleBuilder.CheckIfBuild(abSetting) == false)
                {
                    Selection.activeObject = abSetting;
                    return false;
                }
            }
            return true;
        }

        public static void PrepearBuild(ProjectBuilderSetting setting)
        {
            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            var buildGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

            if (CheckScriptingDefines(buildGroup, setting.IncludeDefines, setting.ExucdeDefines) == false)
            {
                Debug.LogError("宏不正确");
                return;
            }

            if (CheckResources(setting) == false)
            {
                Debug.LogError("AB资源未准备好");
                return;
            }
            SetVersion(setting);
            CopyABToStreamingAssets(setting);
            var hotfixPath = Path.GetFullPath(PathConst.CheckAndFixPathRoot(setting.HotfixPath));
            HotfixBuilder.CreateMainHotfix(setting.MainABSetting.Version, hotfixPath, setting.MainABSetting);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorApplication.ExecuteMenuItem("File/Build Settings...");
        }
        internal static void AddFileToCopyList(string srcPath, string destPath, string sName, string dName, Dictionary<string, CsvFileListInfo> dic)
        {
            File.Copy(srcPath + sName, destPath + dName, true);
            var size = new FileInfo(srcPath + sName).Length / 1024f;
            dic[dName] = new CsvFileListInfo(dName, dName, size);
        }
        internal static void CopyABToStreamingAssets(ProjectBuilderSetting setting)
        {
            var destABPath = PathConst.SourceOSPath;
            if (setting.ClearStreamingAsset && Directory.Exists(destABPath))
                Directory.Delete(destABPath, true);
            if (Directory.Exists(destABPath) == false)
                Directory.CreateDirectory(destABPath);

            var copyFileList = new Dictionary<string, CsvFileListInfo>();
            var mainSrcPath = AssetBundleBuilder.GetExportOSRoot(setting.MainABSetting);
            var mainSrcABPath = mainSrcPath + PathConst.AssetBundlePath;
            var mainFileList = CsvFileListUtil.LoadAtPath(mainSrcPath + PathConst.FileListName);
            foreach (var fileInfo in mainFileList)
            {
                File.Copy(mainSrcABPath + fileInfo.BundleName, destABPath + fileInfo.BundleName, true);
                if (copyFileList.TryGetValue(fileInfo.ID, out var info))
                    if (info.BundleName != fileInfo.BundleName)
                        Debug.LogError("bundle already exist " + info.ID + ":" + info.BundleName + ">" + fileInfo.BundleName);
                copyFileList[fileInfo.ID] = fileInfo;
            }
            AddFileToCopyList(mainSrcPath, destABPath, PathConst.FileListName, PathConst.FileListName, copyFileList);

            foreach (var abSetting in setting.AdditionABSetting)
            {
                var srcOSPath = AssetBundleBuilder.GetExportOSRoot(abSetting);
                var srcABPath = srcOSPath + PathConst.AssetBundlePath;
                var fileList = CsvFileListUtil.LoadAtPath(srcOSPath + PathConst.GetPackFileList(abSetting.PackID));
                foreach (var fileInfo in fileList)
                {
                    File.Copy(srcABPath + fileInfo.BundleName, destABPath + fileInfo.BundleName, true);
                    if (copyFileList.TryGetValue(fileInfo.ID, out var info))
                        if (info.BundleName != fileInfo.BundleName)
                            Debug.LogError("bundle already exist " + info.ID + ":" + info.BundleName + ">" + fileInfo.BundleName);
                    copyFileList[fileInfo.ID] = fileInfo;
                }
                AddFileToCopyList(srcOSPath, destABPath, PathConst.GetPackFileList(abSetting.PackID), PathConst.GetPackFileList(abSetting.PackID), copyFileList);
            }
            CsvFileListUtil.Save(copyFileList, destABPath + PathConst.CopyFileListName);
        }

        internal static void SetVersion(ProjectBuilderSetting setting)
        {
            PlayerSettings.bundleVersion = setting.MainABSetting.Version;
#if UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = setting.AndroidVersionCode;
            var launcherGradlePath = Application.dataPath + "/Plugins/Android/launcherTemplate.gradle";
            if (File.Exists(launcherGradlePath))
            {
                var gradleContent = File.ReadAllText(launcherGradlePath);
                gradleContent = Regex.Replace(gradleContent, @"versionCode \d+", "versionCode " + setting.AndroidVersionCode);
                gradleContent = Regex.Replace(gradleContent, @"versionName '\d+.\d+.\d+'", $"versionName '{setting.MainABSetting.Version}'");
                File.WriteAllText(launcherGradlePath, gradleContent);
            }
#endif
        }

        internal static void CopyStreamingAssetToBuild(ProjectBuilderSetting setting)
        {
            var destPath = EditorUtility.OpenFolderPanel("选择出包资源目录", setting.BuildPath, "");
            if (Directory.Exists(destPath) == false)
                return;
            if (Directory.Exists(PathConst.SourceOSPath) == false)
            {
                EditorUtility.DisplayDialog("错误", PathConst.SourceOSPath + " \n不存在", "OK");
                return;
            }

            var desOSPath = Path.Combine(destPath, PathConst.OSDir);
            Debug.Log("copy to desOSPath " + desOSPath);
            if (Directory.Exists(desOSPath))
                Directory.Delete(desOSPath, true);
            Directory.CreateDirectory(desOSPath);

            var files = Directory.GetFiles(PathConst.SourceOSPath);
            Debug.Log("src " + PathConst.SourceOSPath + "=>" + desOSPath);
            foreach (var file in files)
            {
                var ext = Path.GetExtension(file);
                Debug.Log("asset " + file + "/" + ext);
                if (ext == ".meta") continue;
                File.Copy(file, Path.Combine(desOSPath, Path.GetFileName(file)), true);
            }
            System.Diagnostics.Process.Start(destPath);
        }
    }
}
