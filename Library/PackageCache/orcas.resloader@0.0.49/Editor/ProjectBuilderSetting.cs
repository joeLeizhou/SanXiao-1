using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Orcas.AssetBuilder.Editor;

namespace Orcas.Resources.Editor
{
    public class ProjectBuilderSetting : ScriptableObject
    {
        [InspectorName("打包的AB资源")]
        public AssetBundlesBuilderSetting MainABSetting;
        [InspectorName("默认添加的分包的AB资源")]
        public AssetBundlesBuilderSetting[] AdditionABSetting;
        [InspectorName("需要包含的宏")]
        public string IncludeDefines;
        [InspectorName("需要排除的宏")]
        public string ExucdeDefines;
        [InspectorName("Android Bundle版本号(每次传商店得修改)")]
        public int AndroidVersionCode = 1;
        [InspectorName("热更根目录")]
        public string HotfixPath = "TestServer/";
        [InspectorName("出包目录")]
        public string BuildPath = "../Build/";
        [InspectorName("是否清空streamingasset")]
        public bool ClearStreamingAsset;
    }
}
