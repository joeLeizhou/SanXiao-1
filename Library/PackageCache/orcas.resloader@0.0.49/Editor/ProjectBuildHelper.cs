using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Orcas.AssetBuilder.Editor;
using Orcas.Resources.Editor;
using UnityEditor;
using UnityEngine;

public class ProjectBuildHelper
{
    private static void FindAssets(string functionName, Func<string, bool> callBack)
    {
        var args = System.Environment.GetCommandLineArgs();
        var assetName = "";
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == $@"ProjectBuildHelper.{functionName}")
            {
                assetName = args[i + 1];
                break;
            }
        }
        var uids = AssetDatabase.FindAssets(assetName);
        var output = new StringBuilder();
        for (var i = 0; i < uids.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(uids[i]);
            if (!path.Contains(assetName) || !callBack(path)) continue;
            break;
        }
    }
    
    public static string BuildAB()
    {
        var output = new StringBuilder();
        FindAssets("BuildAB", (string path) =>
        {
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (!(obj is AssetBundlesBuilderSetting setting)) return false;
            AssetBundleBuilder.BuildAssetBundles(setting);
            output.Append(setting.Version);
            return true;
        });
        
        return output.ToString();
    }

    public static void CopyAB2StreamingAssets()
    {
        FindAssets("CopyAB2StreamingAssets", (path) =>
        {
            var obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (!(obj is ProjectBuilderSetting setting)) return false;
            ProjectBuilder.PrepearBuild(setting);
            return true;
        });
    }

    private static void CreateDir(string path)
    {
        if (Directory.Exists(path))
        {
        }
        else
        {
            Directory.CreateDirectory(path);
        }
    }

    private static string BuildAndroid(bool isApk)
    {
        Debug.Log("-----------------------------Build----------------------------------");
        var path = $@"{Application.dataPath}/../../../Build";
        CreateDir(path);
        var scenes = new List<string>();
        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes.Add(EditorBuildSettings.scenes[i].path);
        }

        path +=
            $@"/{Application.productName.Replace(' ', '_')}_V({Application.version.Replace('.', '_')})_{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.";
        if (isApk)
        {
            path += "apk";
        }
        else
        {
            path += "aab";
        }

        var args = Environment.GetCommandLineArgs();
        //var keystoreName = "";
        var keystorePw = "fotoable";
        var keystoreAliasName = "key";
        var keystoreAliasPw = "fotoable";
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == $@"ProjectBuildHelper.BuildAPK" || args[i] == $@"ProjectBuildHelper.BuildAAB" )
            {
                //keystoreName = args[i + 1];
                keystorePw = args[i + 1];
                keystoreAliasName = args[i + 2];
                keystoreAliasPw = args[i + 3];
                break;
            }
        }

        PlayerSettings.Android.useCustomKeystore = true;
        //PlayerSettings.Android.keystoreName = keystoreName;
        PlayerSettings.Android.keyaliasName = keystoreAliasName;
        PlayerSettings.keystorePass = keystorePw;
        PlayerSettings.keyaliasPass = keystoreAliasPw;
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.androidBuildType = isApk ? AndroidBuildType.Debug : AndroidBuildType.Release;
        EditorUserBuildSettings.buildAppBundle = !isApk;
        BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            scenes = scenes.ToArray(),
            target = BuildTarget.Android,
            locationPathName = path,
            targetGroup = BuildTargetGroup.Android
        });

        return path;
    }

    public static void BuildXCode()
    {
        Debug.Log("-----------------------------Build----------------------------------");
        var path = $@"{Application.dataPath}/../Build";
        CreateDir(path);
        var scenes = new List<string>();
        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes.Add(EditorBuildSettings.scenes[i].path);
        }

        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == $@"ProjectBuildHelper.BuildXCode" || args[i] == $@"ProjectBuildHelper.BuildAAB")
            {
                break;
            }
        }

        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
        BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            scenes = scenes.ToArray(),
            target = BuildTarget.iOS,
            locationPathName = path,
            targetGroup = BuildTargetGroup.iOS
        });
    }

    public static string BuildAPK()
    {
        return BuildAndroid(true);
    }
    
    public static string BuildAAB()
    {
        return BuildAndroid(false);
    }
}
