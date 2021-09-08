using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace Orcas.AssetBuilder.Editor
{
    public enum BuildType : byte
    {
        [InspectorName("单文件打成一个包")]
        SingleFile = 0,
        [InspectorName("按目录打成一个包")]
        Directory = 1,
        [InspectorName("所有同类文件打成一个包")]
        AllInOne = 2
    }

    [Serializable]
    public class BuildMethod
    {
        [InspectorName("原始文件后缀")]
        public string OriginalExtension = ".";
        [InspectorName("文件打包方式")]
        public BuildType BuildType = BuildType.SingleFile;
        [InspectorName("包名（AllInOne启用）")]
        public string BundleName;
        [InspectorName("是否先改成二进制文件再打包")]
        public bool ReNameAsBinary;
        [InspectorName("是否需要加密")]
        public bool NeedEncrypt;
        [InspectorName("加密秘钥")]
        public string EncryptKey;
        [InspectorName("生成AB后缀")]
        public string ExportExtension;
    }
    public class AssetBundlesBuilderSetting : ScriptableObject
    {
        [InspectorName("PackID")]
        public int PackID = 0;
        [InspectorName("打包目录")]
        public string[] Roots;
        [InspectorName("输出目录")]
        public string ExportRoot = "AssetBundles/";
        [InspectorName("是否输出到对应版本号的目录下")]
        public bool VersionExport = false;
        [InspectorName("版本号")]
        public string Version = "0.0.0";
        [InspectorName("是否打增量包(需要保留之前版本的FileList)")]
        public bool ExportAdditionalPack = false;
        [InspectorName("上一个版本号(增量包使用)")]
        public string OldVersion = "0.0.0";
        public BuildMethod[] BuildMethods = new BuildMethod[]
        {
            new BuildMethod{ OriginalExtension = ".prefab" },
            new BuildMethod{ OriginalExtension = ".png" },
            new BuildMethod{ OriginalExtension = ".jpg" },
            new BuildMethod{ OriginalExtension = ".psd" },
            new BuildMethod{ OriginalExtension = ".hdr" },
            new BuildMethod{ OriginalExtension = ".mat" },
            new BuildMethod{ OriginalExtension = ".unity" },
            new BuildMethod{ OriginalExtension = ".mp3" },
            new BuildMethod{ OriginalExtension = ".ogg" },
            new BuildMethod{ OriginalExtension = ".tga" },
            new BuildMethod{ OriginalExtension = ".wav" },
            new BuildMethod{ OriginalExtension = ".json" },
            new BuildMethod{ OriginalExtension = ".csv" },
            new BuildMethod{ OriginalExtension = ".ttf" },
            new BuildMethod{ OriginalExtension = ".physicmaterial" },
            new BuildMethod{ OriginalExtension = ".anim" },
            new BuildMethod{ OriginalExtension = ".playable" },
            new BuildMethod{ OriginalExtension = ".otf" },
            new BuildMethod{ OriginalExtension = ".controller" },
            new BuildMethod{ OriginalExtension = ".asset" },
            new BuildMethod{ OriginalExtension = ".bytes" },
            new BuildMethod{ OriginalExtension = ".tif" },
            new BuildMethod{ OriginalExtension = ".fbx" },
            new BuildMethod{ OriginalExtension = ".exr" },
            new BuildMethod{ OriginalExtension = ".shader", BuildType = BuildType.AllInOne, BundleName = "shaders" },
            new BuildMethod{ OriginalExtension = ".shadersubgraph", BuildType = BuildType.AllInOne, BundleName = "shadersubgraph" },
            new BuildMethod{ OriginalExtension = ".shadergraph", BuildType = BuildType.AllInOne, BundleName = "shadergraph" },
            new BuildMethod{ OriginalExtension = ".lua", BuildType = BuildType.Directory, ReNameAsBinary = true, ExportExtension = ".luabundle"}
        };
        [InspectorName("打包选项")]
        public BuildAssetBundleOptions Options = BuildAssetBundleOptions.None | BuildAssetBundleOptions.DeterministicAssetBundle;
        [InspectorName("打包之前是否清空包名")]
        public bool PreClearAssetBundlesName = true;
        [InspectorName("打包之后是否清空包名")]
        public bool AfterClearAssetBundlesName = true;
        [InspectorName("是否清空临时打包目录(重复打同一个包可以不清空)")]
        public bool PerClearTempDir = true;
        [InspectorName("是否添加hash到包名")]
        public bool UseAppendHashName = true;
        [InspectorName("是否检查并标记被重复依赖的资源")]
        public bool CheckDependentAssets = true;
    }
}