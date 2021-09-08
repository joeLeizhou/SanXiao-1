using System;
using UnityEditor;
using UnityEngine;
using Orcas.AssetBuilder;
using Orcas.AssetBuilder.Editor;
using Orcas.Csv;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Orcas.Resources.Editor
{
    public static class HotfixBuilder
    {
        internal static void BuildHotfix(HotfixBuilderSetting setting)
        {
            if (setting.ForceRebuild || AssetBundleBuilder.CheckIfBuild(setting.HotfixABSetting) == false)
            {
                AssetBundleBuilder.BuildAssetBundles(setting.HotfixABSetting);
            }
            var buildV = new HotfixVersion(setting.MainABSetting.Version);
            var hotV = new HotfixVersion(setting.HotfixABSetting.Version);
            var hotfixFullPath = Path.GetFullPath(PathConst.CheckAndFixPathRoot(setting.HotfixPath));
            var versionPath = PathConst.GetHotVersionPath(hotfixFullPath, buildV);
            if (File.Exists(versionPath) == false)
            {
                if (AssetBundleBuilder.CheckIfBuild(setting.MainABSetting) == false)
                {
                    EditorUtility.DisplayDialog("错误", "大版本资源列表不存在", "确认");
                    return;
                }
                CreateMainHotfix(setting.MainABSetting.Version, hotfixFullPath, setting.MainABSetting);
            }

            var destOSPath = PathConst.GetHotfixOSVersionPath(hotfixFullPath, hotV, buildV);
            var destABPath = PathConst.GetHotfixOSBundlePath(hotfixFullPath, buildV);
            Debug.Log("dest ab path " + destABPath);
            if (Directory.Exists(destABPath) == false)
                Directory.CreateDirectory(destABPath);
            if (Directory.Exists(destOSPath) == false)
                Directory.CreateDirectory(destOSPath);

            var versions = LoadVersion(hotfixFullPath, buildV);
            var lastVersion = new HotfixVersion(versions[versions.Count - 1].ID);
            var isRebuild = lastVersion == hotV;
            if (setting.HotfixType == HotfixBuildType.CompareToLast)
            {
                if (isRebuild)
                    lastVersion = new HotfixVersion(versions[versions.Count - 2].ID);
            }
            else if (setting.HotfixType == HotfixBuildType.CompareToMain)
            {
                lastVersion = buildV;
            }

            var srcOSPath = AssetBundleBuilder.GetExportOSRoot(setting.HotfixABSetting);
            var srcABPath = srcOSPath + PathConst.AssetBundlePath;
            var srcFileList = CsvFileListUtil.LoadDicAtPath(srcOSPath + PathConst.FileListName);
            var lastHotfixOSPath = PathConst.GetHotfixOSVersionPath(hotfixFullPath, lastVersion, buildV);
            var lastAllFileList = CsvFileListUtil.LoadDicAtPath(lastHotfixOSPath + PathConst.AllFileListName);
            var newAllFileList = new Dictionary<string, CsvFileListInfo>(lastAllFileList);
            // find update files
            var diffFileSize = 0.0f;
            var diffFileList = new Dictionary<string, CsvFileListInfo>();
            foreach (var fileInfo in srcFileList)
            {
                if (lastAllFileList.TryGetValue(fileInfo.Key, out var value))
                {
                    if (value.BundleName != fileInfo.Value.BundleName)
                    {
                        diffFileSize += fileInfo.Value.Size;
                        diffFileList.Add(fileInfo.Key, fileInfo.Value);
                    }
                    else if (setting.CopyManifeist && value.ID == PathConst.ManifestName)
                    {
                        diffFileSize += fileInfo.Value.Size;
                        diffFileList.Add(fileInfo.Key, fileInfo.Value);
                    }
                }
                else
                {
                    diffFileSize += fileInfo.Value.Size;
                    diffFileList.Add(fileInfo.Key, fileInfo.Value);
                }
                newAllFileList[fileInfo.Key] = fileInfo.Value;
            }
            // copy files
            foreach (var fileInfo in diffFileList)
            {
                File.Copy(srcABPath + fileInfo.Value.BundleName, destABPath + fileInfo.Value.BundleName, true);
            }
            // save version
            CsvFileListUtil.Save(diffFileList, destOSPath + PathConst.FileListName);
            CsvFileListUtil.Save(newAllFileList, destOSPath + PathConst.AllFileListName);
            if (setting.HotfixType == HotfixBuildType.CompareToLast)
            {
                if (isRebuild)
                    versions[versions.Count - 2].HotVer = hotV.ToString();
                else
                    versions[versions.Count - 1].HotVer = hotV.ToString();
            }
            else if (setting.HotfixType == HotfixBuildType.CompareToMain)
            {
                for (int i = 0; i < versions.Count; i++)
                {
                    versions[i].HotVer = hotV.ToString();
                }
            }
            if (isRebuild == false)
                versions.Add(new CsvHotfix(hotV.ToString(), hotV.ToString(), diffFileSize, false));
            else
                versions[versions.Count - 1].Size = diffFileSize;
            SaveVersion(versions, buildV, hotfixFullPath);

            System.Diagnostics.Process.Start(destOSPath);
        }

        internal static List<CsvHotfix> LoadVersion(string root, HotfixVersion buildV)
        {
            var path = PathConst.GetHotVersionPath(Path.GetFullPath(root), buildV);
            var versions = new List<CsvHotfix>();
            if (File.Exists(path))
            {
                Debug.Log("load version " + path);
                var table = CsvLoader<string>.Import<CsvHotfix>(File.ReadAllText(path));
                foreach (var data in table)
                {
                    versions.Add(data.Value as CsvHotfix);
                }
            }
            versions.Sort((a, b) => (new HotfixVersion(a.ID).GetHashCode() - (new HotfixVersion(b.ID)).GetHashCode()));
            return versions;
        }

        internal static void SaveVersion(List<CsvHotfix> versions, HotfixVersion buildV, string root)
        {
            var path = PathConst.GetHotVersionPath(root, buildV);
            var sb = new StringBuilder("");
            sb.AppendLine("ID,HotVer,Size,ForceUpdate");
            sb.AppendLine("string,string,float,bool");
            sb.AppendLine(",,,");
            foreach (var hotfix in versions)
            {
                sb.AppendLine($"{hotfix.ID},{hotfix.HotVer},{hotfix.Size:F3},{(hotfix.ForceUpdate ? 1 : 0)}");
            }
            Debug.Log("save version " + path);
            File.WriteAllText(path, sb.ToString(), System.Text.Encoding.UTF8);
        }

        internal static void CreateMainHotfix(string buildVersion, string hotfixPath, AssetBundlesBuilderSetting setting)
        {
            var csvHotfix = new CsvHotfix(buildVersion, buildVersion, 0, false);
            var buildV = new HotfixVersion(buildVersion);
            var srcPath = AssetBundleBuilder.GetExportOSRoot(setting);
            var destPath = PathConst.GetHotfixOSVersionPath(hotfixPath, buildV, buildV);
            if (Directory.Exists(destPath) == false)
                Directory.CreateDirectory(destPath);

            Debug.Log("copy " + srcPath + PathConst.FileListName + " to " + destPath);
            File.Copy(srcPath + PathConst.FileListName, destPath + PathConst.AllFileListName, true);
            SaveVersion(new List<CsvHotfix> { csvHotfix }, buildV, hotfixPath);
        }
    }
}
