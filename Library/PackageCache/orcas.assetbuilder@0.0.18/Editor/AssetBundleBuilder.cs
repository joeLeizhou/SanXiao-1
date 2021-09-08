using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Orcas.Core.Tools;
using UnityEditor;
using UnityEngine;

namespace Orcas.AssetBuilder.Editor
{
    public partial class AssetBundleBuilder
    {
        internal static readonly string FileListName = "FileList.csv";
        internal static readonly string AssetBundleName = "AssetBundle/";

        /// <summary>
        /// 打包的临时目录
        /// </summary>
        internal static readonly string TempDir = Path.GetFullPath("AssetBundlesTemp/");
        /// <summary>
        /// 临时的转化成bytes文件的目录
        /// </summary>
        internal static readonly string TempPackageDir = "Assets/BuildABTempDir/";
        internal static string[] Roots;
        internal static Dictionary<string, BuildMethod> TempBuildMethods;
        internal static readonly HashSet<string> TempBinarySet = new HashSet<string>();
        public static void Init(AssetBundlesBuilderSetting setting)
        {
            TempBuildMethods = new Dictionary<string, BuildMethod>();
            for (int i = 0; i < setting.BuildMethods.Length; i++)
            {
                TempBuildMethods.Add(setting.BuildMethods[i].OriginalExtension, setting.BuildMethods[i]);
            }
            Roots = new List<string>(setting.Roots).ToArray();
        }
        public static void BuildAssetBundles(AssetBundlesBuilderSetting setting)
        {
            Init(setting);
            Debug.Log("==============>准备打包");
            Debug.Log("目标平台: " + OSDir + " ,Pack " + setting.PackID);
            if (setting.PreClearAssetBundlesName)
            {
                Debug.Log("==============>开始移除AssetBundle命名");
                ClearAssetBundlesName();
            }
            Debug.Log("==============>将需要加密的文件加密");
            TryEncryptKey(setting);
            Debug.Log("==============>将unity不兼容格式进行后缀转化");
            CopyFileAsBinary(setting);
            try
            {
                Debug.Log("==============>开始重新命名AssetBundle");
                MarkAssetBundlesNames(setting);
                if (setting.CheckDependentAssets)
                {
                    Debug.Log("==============>检查被多次依赖的资源");
                    MarkDependencyAssetBundlesName(setting);
                }
                Debug.Log("==============>开始打包");
                if (setting.PerClearTempDir)
                {
                    Debug.Log("==============>清空" + TempDir);
                    Utils.ClearOrCreateDirectory(TempDir);
                }
                var buildTarget = EditorUserBuildSettings.activeBuildTarget;
                EditorUtility.DisplayProgressBar("Build AssetBundles", " " + buildTarget, 0.2f);
                var manifest = BuildPipeline.BuildAssetBundles(TempDir, setting.Options, buildTarget);
                Debug.Log("==============>生成FileList.csv");
                GenerateFileList(manifest, setting);
                Debug.Log("==============>移动包体到目标目录");
                CopyFileFromTempDirToExportDir(manifest, setting);
                if (setting.VersionExport && setting.ExportAdditionalPack)
                {
                    Debug.Log("==============>开始打增量包");
                    ExportAdditionalPack(setting);
                }
                Debug.Log("==============>资源打包完成");
            }
            finally
            {
                if (setting.AfterClearAssetBundlesName)
                {
                    Debug.Log("==============>移除AssetBundleName");
                    ClearAssetBundlesName();
                }
                Debug.Log("==============>还原被加密的文件");
                TryDecryptKey(setting);
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        internal static void RevertBuildAssetBundles(AssetBundlesBuilderSetting setting)
        {
            Init(setting);
            {
                if (setting.AfterClearAssetBundlesName)
                {
                    Debug.Log("==============>移除AssetBundleName");
                    ClearAssetBundlesName();
                }
                Debug.Log("==============>还原被加密的文件");
                TryDecryptKey(setting);
                Debug.Log("==============>删除临时目录");
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        private static void ExportAdditionalPack(AssetBundlesBuilderSetting setting)
        {
            var newVersionPath = GetExportOSRoot(setting);
            var fileList = CsvFileListUtil.LoadAtPath(newVersionPath + FileListName);
            var oldVersionPath = $"{newVersionPath}../{setting.OldVersion}/";
            var additionalPath = $"{newVersionPath}../{setting.Version}_additional/";
            Utils.ClearOrCreateDirectory(additionalPath);

            var oldFileList = CsvFileListUtil.LoadAtPath(oldVersionPath + FileListName);
            var oldMd5Set = new HashSet<string>();
            for (var i = 0; i < oldFileList.Count; i++)
            {
                oldMd5Set.Add(oldFileList[i].BundleName);
            }

            var newFileList = new List<CsvFileListInfo>();
            for (var i = 0; i < fileList.Count; i++)
            {
                if (oldMd5Set.Contains(fileList[i].BundleName) == false)
                {
                    newFileList.Add(fileList[i]);
                    FileUtil.CopyFileOrDirectory(newVersionPath + fileList[i].BundleName, additionalPath + fileList[i].BundleName);
                }
            }
            CsvFileListUtil.Save(newFileList, additionalPath + FileListName);
        }

        private static void CopyFileAsBinary(AssetBundlesBuilderSetting setting)
        {
            TempBinarySet.Clear();
            for (var i = 0; i < Roots.Length; i++)
            {
                DfsPath(Roots[i], (path) =>
                {
                    var ext = Path.GetExtension(path).ToLower();
                    if (TempBuildMethods.TryGetValue(ext, out var buildMethod))
                    {
                        if (buildMethod.ReNameAsBinary)
                        {
                            TempBinarySet.Add(GetPathInProject(path) + ".bytes");
                            File.Copy(path, path + ".bytes", true);
                        }
                    }
                });
            }
            AssetDatabase.Refresh();
        }

        private static void TryEncryptKey(AssetBundlesBuilderSetting setting)
        {
            for (var i = 0; i < Roots.Length; i++)
            {
                DfsPath(Roots[i], (path) =>
                {
                    var ext = Path.GetExtension(path).ToLower();
                    if (TempBuildMethods.TryGetValue(ext, out var buildMethod))
                    {
                        if (buildMethod.NeedEncrypt)
                        {
                            SecurityUtils.Encrypt(path, buildMethod.EncryptKey.ToCharArray());
                        }
                    }
                });
            }
        }

        private static void TryDecryptKey(AssetBundlesBuilderSetting setting)
        {
            for (var i = 0; i < Roots.Length; i++)
            {
                DfsPath(Roots[i], (path) =>
                {
                    var ext = Path.GetExtension(path).ToLower();
                    if (TempBuildMethods.TryGetValue(ext, out var buildMethod))
                    {
                        if (buildMethod.NeedEncrypt)
                        {
                            SecurityUtils.Decrypt(path, buildMethod.EncryptKey.ToCharArray());
                        }
                    }
                });
            }
        }

        private static string GetSaveBundleName(bool useHash, string bundle, AssetBundleManifest manifest)
        {
            string subfix = useHash ? manifest.GetAssetBundleHash(bundle).ToString() : Utils.GenerateMD5File(TempDir + bundle);
            return bundle.Replace('.', '_') + "_" + subfix;
        }

        private static void CopyFileFromTempDirToExportDir(AssetBundleManifest manifest, AssetBundlesBuilderSetting setting)
        {
            var exportABPath = GetExportOSRoot(setting) + AssetBundleName;
            if (!Directory.Exists(exportABPath))
                Directory.CreateDirectory(exportABPath);
            var bundles = manifest.GetAllAssetBundles();
            for (int i = 0; i < bundles.Length; i++)
            {
                var bundle = bundles[i];
                FileUtil.CopyFileOrDirectory(TempDir + bundle, exportABPath + GetSaveBundleName(setting.UseAppendHashName, bundle, manifest));
                EditorUtility.DisplayProgressBar("Copy Files To Export Path", bundle, (float)i / bundles.Length);
            }
            var manifestSourceName = Path.GetFileName(Path.GetDirectoryName(TempDir));
            FileUtil.CopyFileOrDirectory(TempDir + manifestSourceName, exportABPath + GetPackManifest(setting.PackID));
        }

        private static void GenerateFileList(AssetBundleManifest manifest, AssetBundlesBuilderSetting setting)
        {
            var path = GetExportOSRoot(setting);
            Utils.ClearOrCreateDirectory(path);
            EditorUtility.DisplayProgressBar("Generate FileList", "", 0.5f);
            var bundles = manifest.GetAllAssetBundles();
            var fileList = new List<CsvFileListInfo>(bundles.Length);
            foreach (var bundle in bundles)
            {
                var filePath = TempDir + bundle;
                var file = new FileInfo(filePath);
                var bundleName = GetSaveBundleName(setting.UseAppendHashName, bundle, manifest);
                fileList.Add(new CsvFileListInfo(bundle, bundleName, (float)file.Length / 1024));
            }
            var manifeistSize = new FileInfo(TempDir + Path.GetFileName(Path.GetDirectoryName(TempDir))).Length;
            var manifestName = GetPackManifest(setting.PackID);
            fileList.Add(new CsvFileListInfo(manifestName, manifestName, (float)manifeistSize / 1024));
            CsvFileListUtil.Save(fileList, path + FileListName);
        }

        public static bool CheckIfBuild(AssetBundlesBuilderSetting setting)
        {
            var path = GetExportOSRoot(setting) + FileListName;
            return File.Exists(path);
        }
        private static string GetPathInProject(string path)
        {
            var assetIndex = path.LastIndexOf(Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
            if (assetIndex > 0)
                path = path.Substring(assetIndex + 1);
            else
            {
                var packageIndex = path.LastIndexOf(Path.DirectorySeparatorChar + "Packages" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
                if (packageIndex > 0)
                    path = path.Substring(packageIndex + 1);
            }
            return path;
        }

        public static string[] GetAssetBundlesNames(AssetBundlesBuilderSetting setting)
        {
            var result = new List<string>();
            for (var i = 0; i < setting.Roots.Length; i++)
            {
                DfsPath(setting.Roots[i], (path) =>
                {
                    var bundleName = MarkAssetBundlesName(path, setting, true);
                    if (!string.IsNullOrEmpty(bundleName))
                        result.Add(bundleName);
                });
            }
            return result.ToArray();
        }

        public static void MarkAssetBundlesNames(AssetBundlesBuilderSetting setting)
        {
            if (Roots == null) return;
            for (var i = 0; i < Roots.Length; i++)
            {
                DfsPath(Roots[i], (path) =>
                {
                    MarkAssetBundlesName(path, setting);
                });
            }
        }

        public static string MarkAssetBundlesName(string path, AssetBundlesBuilderSetting setting, bool onlyName = false)
        {
            path = GetPathInProject(path);
            string ext;
            if (TempBinarySet.Contains(path))
            {
                ext = Path.GetExtension(Path.GetFileNameWithoutExtension(path)).ToLower();
            }
            else
            {
                ext = Path.GetExtension(path).ToLower();
            }

            if (TempBuildMethods.TryGetValue(ext, out var buildMethod))
            {
                var importer = AssetImporter.GetAtPath(path);
                if (importer == null)
                {
                    Debug.LogError("importer filePath:" + path);
                }
                else
                {
                    EditorUtility.DisplayProgressBar("Mark AssetBundle Names", importer.assetPath, 0.2f);
                    var extension = buildMethod.ExportExtension.Trim();
                    var bundleName = importer.assetBundleName;
                    switch (buildMethod.BuildType)
                    {
                        case BuildType.SingleFile:
                            {
                                bundleName = Path.GetFileNameWithoutExtension(path) + extension;
                                break;
                            }
                        case BuildType.Directory:
                            {
                                bundleName = Path.GetFileName(Path.GetDirectoryName(path)) + extension;
                                break;
                            }
                        case BuildType.AllInOne:
                            {
                                bundleName = buildMethod.BundleName + extension;
                                break;
                            }
                    }

                    bundleName = bundleName.ToLower();
                    if (!importer.assetBundleName.Equals(bundleName, StringComparison.Ordinal) && !onlyName)
                    {
                        importer.assetBundleName = bundleName;
                    }
                    return bundleName;
                }
            }
            return null;
        }

        public static void MarkDependencyAssetBundlesName(AssetBundlesBuilderSetting setting)
        {
            if (Roots == null) return;
            var depCounts = new Dictionary<string, int>();
            for (var i = 0; i < Roots.Length; i++)
            {
                DfsPath(Roots[i], (path) =>
                {
                    path = GetPathInProject(path);
                    var deps = AssetDatabase.GetDependencies(path, true);
                    for (int j = 0; j < deps.Length; j++)
                    {
                        if (depCounts.ContainsKey(deps[j]))
                            depCounts[deps[j]] = depCounts[deps[j]] + 1;
                        else
                            depCounts[deps[j]] = 1;
                    }
                });
            }
            foreach (var item in depCounts)
            {
                if (item.Value > 1)
                {
                    if (item.Key.StartsWith("Assets/", StringComparison.Ordinal) == false)
                    {
                        if (item.Key.EndsWith(".cs", StringComparison.Ordinal) == false)
                            Debug.LogError($"path:{item.Key} count:{item.Value}");
                    }
                    else
                    {
                        Debug.Log($"path:{item.Key} count:{item.Value}");
                        MarkAssetBundlesName(item.Key, setting);
                    }
                }
            }
        }

        public static void CheckDependentNotInAssets(AssetBundlesBuilderSetting setting)
        {
            if (setting.Roots == null)
                return;
            var depCounts = new Dictionary<string, List<string>>();
            for (int i = 0; i < setting.Roots.Length; i++)
            {
                DfsPath(setting.Roots[i], (path) =>
                {
                    path = GetPathInProject(path);
                    var deps = AssetDatabase.GetDependencies(path, true);
                    for (int j = 0; j < deps.Length; j++)
                    {
                        var depPath = deps[j];
                        if (depPath.StartsWith("Assets/", StringComparison.Ordinal) == false && depPath.EndsWith(".cs", StringComparison.Ordinal) == false)
                        {
                            if (depCounts.ContainsKey(depPath))
                                depCounts[depPath].Add(path);
                            else
                                depCounts.Add(depPath, new List<string>() { path });
                        }
                    }
                });
            }
            foreach (var item in depCounts)
            {
                Debug.LogError(item.Key + ":" + item.Value.Count + "\n" + string.Join("\n", item.Value));
            }
        }
    }
}