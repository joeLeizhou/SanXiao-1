using Orcas.AssetBuilder;
using Orcas.AssetBuilder.Editor;
using Orcas.Resources;
using Orcas.Resources.Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PackExporter
{
    public static void ExportPacks(PackExportSetting setting)
    {
        var allABSetting = Object.Instantiate(setting.ABSettings[0]);
        var abAllPaths = new Dictionary<string, int>();
        foreach (var abSetting in setting.ABSettings)
        {
            foreach (var path in abSetting.Roots)
            {
                abAllPaths[path] = 1;
            }
        }
        allABSetting.Roots = new string[abAllPaths.Keys.Count];
        abAllPaths.Keys.CopyTo(allABSetting.Roots, 0);
        if (setting.ForceRebuild)
        {
            AssetBundleBuilder.BuildAssetBundles(allABSetting);
        }
        ExportAllPackAB(allABSetting, setting.ExportRoot);

        for (int i = 0; i < setting.ABSettings.Length; i++)
        {
            var abSetting = setting.ABSettings[i];
            EditorUtility.DisplayProgressBar("导出分包配置", "Pack " + abSetting.PackID, (float)i / setting.ABSettings.Length);
            ExportPackConfig(allABSetting, abSetting, setting.ExportRoot);
        }
        AssetBundle.UnloadAllAssetBundles(true);
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ExportAllPackAB(AssetBundlesBuilderSetting setting, string destRoot)
    {
        Debug.Log("开始导出分包AB");
        var destOSRoot = PathConst.GetPackOSPath(Path.GetFullPath(destRoot));
        var destABPath = destOSRoot + PathConst.AssetBundlePath;
        var srcOSPath = AssetBundleBuilder.GetExportOSRoot(setting);
        var srcABPath = srcOSPath + PathConst.AssetBundlePath;
        var fileList = CsvFileListUtil.LoadAtPath(srcOSPath + PathConst.FileListName);
        if (Directory.Exists(destABPath) == false)
            Directory.CreateDirectory(destABPath);
        for (int i = 0; i < fileList.Count; i++)
        {
            var fileInfo = fileList[i];
            EditorUtility.DisplayProgressBar("导出分包AB", fileInfo.ID, (float)i / fileList.Count);
            File.Copy(srcABPath + fileInfo.BundleName, destABPath + fileInfo.BundleName, true);
        }
        Debug.Log("结束导出分包AB");
    }

    private static void ExportPackConfig(AssetBundlesBuilderSetting allSetting, AssetBundlesBuilderSetting setting, string destRoot)
    {
        Debug.Log("开始导出分包" + setting.PackID);
        AssetBundle.UnloadAllAssetBundles(true);
        var allSrcOSPath = AssetBundleBuilder.GetExportOSRoot(allSetting);
        var allSrcABPath = allSrcOSPath + PathConst.AssetBundlePath;
        var allABManifestPath = allSrcABPath + AssetBundleBuilder.GetPackManifest(allSetting.PackID);
        var destOSRoot = PathConst.GetPackOSPath(Path.GetFullPath(destRoot));
        var destABPath = destOSRoot + PathConst.AssetBundlePath;
        if (!Directory.Exists(destABPath))
            Directory.CreateDirectory(destABPath);
        var allFileList = CsvFileListUtil.LoadDicAtPath(allSrcOSPath + PathConst.FileListName);
        var allManifest = AssetBundle.LoadFromFile(allABManifestPath).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        AssetBundleBuilder.Init(setting);
        var packBundleNames = AssetBundleBuilder.GetAssetBundlesNames(setting);
        var fileLists = new List<CsvFileListInfo>(packBundleNames.Length);
        var bundleInfos = new List<PackAssetBundleInfo>(packBundleNames.Length);
        for (int i = 0; i < packBundleNames.Length; i++)
        {
            fileLists.Add(allFileList[packBundleNames[i]]);
            bundleInfos.Add(new PackAssetBundleInfo(packBundleNames[i], allManifest.GetAssetBundleHash(packBundleNames[i]), allManifest.GetDirectDependencies(packBundleNames[i])));
        }

        var customManifest = new PackAssetBundleManifest();
        customManifest.AssetBundleInfos = bundleInfos.ToArray();
        var customManifestName = PathConst.GetCustomPackManifest(setting.PackID);
        var manifestJson = JsonUtility.ToJson(customManifest, true);
        var customManifestName1 = customManifestName.Replace('.', '_') + "_" + manifestJson.GetHashCode().ToString("x8");
        File.WriteAllText(allSrcABPath + customManifestName1, manifestJson);
        File.WriteAllText(destABPath + customManifestName1, manifestJson);

        fileLists.Add(new CsvFileListInfo(customManifestName, customManifestName1, (float)manifestJson.Length / 1024));
        CsvFileListUtil.Save(fileLists, allSrcOSPath + PathConst.GetPackFileList(setting.PackID));
        CsvFileListUtil.Save(fileLists, destOSRoot + PathConst.GetPackFileList(setting.PackID));
        Debug.Log("结束导出分包" + setting.PackID);
    }

    public static void DeleveUnuseFile(PackExportSetting setting)
    {
        string destRoot = Path.GetFullPath(setting.ExportRoot);
        var destOSPath = PathConst.GetPackOSPath(destRoot);
        var destABPath = destOSPath + PathConst.AssetBundlePath;
        if (Directory.Exists(destABPath) == false)
        {
            Debug.LogError(destABPath + " not exist");
            return;
        }
        Debug.Log("删除无用文件开始");
        var fileListPaths = Directory.GetFiles(destOSPath);
        var inFileListAll = new HashSet<string>();
        foreach (var fileListPath in fileListPaths)
        {
            var fileList = CsvFileListUtil.LoadAtPath(fileListPath);
            foreach (var info in fileList)
            {
                inFileListAll.Add(info.BundleName);
            }
        }

        var abPaths = Directory.GetFiles(destABPath);
        for (int i = 0; i < abPaths.Length; i++)
        {
            var abName = Path.GetFileName(abPaths[i]);
            if (inFileListAll.Contains(abName) == false)
            {
                EditorUtility.DisplayProgressBar("删除不用文件", abName, (float)i / abPaths.Length);
                File.Delete(abPaths[i]);
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("删除无用文件结束");
    }
}
