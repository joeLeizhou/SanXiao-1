using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
public static class OrcasCoreMenu
{

    #region Quick Open Folder
    
    [MenuItem("Orcas/Quick Open Folder/iOS Plugins")]
    public static void QuickOpenIOSPlugins()
    {
        string dstFullPath = Path.GetFullPath("Assets/Plugins/iOS");
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在Plugins/iOS目录！");
        }
    }

    [MenuItem("Orcas/Quick Open Folder/Android Plugins")]
    public static void QuickOpenAndroidPlugins()
    {
        string dstFullPath = Path.GetFullPath("Assets/Plugins/Android");
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在Plugins/Android目录！");
        }
    }
    
    [MenuItem("Orcas/Quick Open Folder/External")]
    public static void QuickOpenExternal()
    {
        string dstFullPath = Path.GetFullPath("Assets/External");
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在External目录！");
        }
    }
    
    [MenuItem("Orcas/Quick Open Folder/Application.persistentDataPath")]
    public static void QuickOpenPersistent()
    {
        string dstFullPath = Application.persistentDataPath;
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在Application.persistentDataPath目录！");
        }
    }
    
    
    [MenuItem("Orcas/Quick Open Folder/Application.temporaryCachePath")]
    public static void QuickOpenTemporaryCachePath()
    {
        string dstFullPath = Application.temporaryCachePath;
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在Application.temporaryCachePath！");
        }
    }
    
    [MenuItem("Orcas/Quick Open Folder/Application.streamingAssetsPath")]
    public static void QuickOpenStreamingAssetsPath()
    {
        string dstFullPath = Application.streamingAssetsPath;
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在Application.streamingAssetsPath！");
        }
    }
    
    [MenuItem("Orcas/Quick Open Folder/Application.dataPath")]
    public static void QuickOpenDataPath()
    {
        string dstFullPath = Application.dataPath;
        if (Directory.Exists(dstFullPath))
        {
            EditorUtility.RevealInFinder(dstFullPath);
        }
        else
        {
            Debug.LogError("不存在Application.dataPath！");
        }
    }
    #endregion
    
    [MenuItem ("Orcas/Game Data Management/Clear Preferences")]
    static void ClearPreferences () 
    {
        PlayerPrefs.DeleteAll();
    }
    
    
    [MenuItem("Orcas/生成安卓Gradle和Manifest模板", false, 1)]
    public static void CopyGradleAndManifestTemplate()
    {
        string dstFullPath = Path.GetFullPath("Assets/Plugins/Android");
        bool dirExist = Directory.Exists(dstFullPath);
        
        if (!dirExist)
        {
            Directory.CreateDirectory(dstFullPath);
        }

        string[] copyFileName = new[] {"AndroidManifest.xml", "mainTemplate.gradle", "launcherTemplate.gradle"};
        string[] srcPath = new string[copyFileName.Length];
        string[] dstPath = new string[copyFileName.Length];        
        string allExistFile = "";
        for (int i = 0; i < copyFileName.Length; i++)
        {
            srcPath[i] = Path.GetFullPath("Packages/orcas.core/Editor/Template/" + copyFileName[i]);
            dstPath[i] = Path.GetFullPath("Assets/Plugins/Android/" + copyFileName[i]);
            allExistFile += (copyFileName[i] + ", ");
        }

        if (allExistFile.Length > 0)
        {
            string msg = "当前Plugins/Android目录下面存在" + allExistFile + "操作将会覆盖文件。\n是否确定要覆盖文件？";
            if (EditorUtility.DisplayDialog("警告", msg, "确定", "取消"))
            {
                for (int i = 0; i < srcPath.Length; i++)
                {
                    File.Copy(srcPath[i], dstPath[i], true);    
                }
                AssetDatabase.Refresh();
                Debug.Log("覆盖Gradle和Manifest完成");
            }
        }
        else
        {
            for (int i = 0; i < srcPath.Length; i++)
            {
                File.Copy(srcPath[i], dstPath[i], true);    
            }
            AssetDatabase.Refresh();
            Debug.Log("拷贝Gradle和Manifest完成");
        }
    }
}
