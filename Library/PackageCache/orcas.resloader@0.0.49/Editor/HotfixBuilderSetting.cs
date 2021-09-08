using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Orcas.AssetBuilder.Editor;

namespace Orcas.Resources.Editor
{
    [Serializable]
    public enum HotfixBuildType
    {
        [InspectorName("跟大版本对比")]
        CompareToMain = 0,
        [InspectorName("跟上一版本对比")]
        CompareToLast = 1
    }
    public class HotfixBuilderSetting : ScriptableObject
    {
        [InspectorName("大版本AB资源配置")]
        public AssetBundlesBuilderSetting MainABSetting;
        [InspectorName("热更的AB资源配置")]
        public AssetBundlesBuilderSetting HotfixABSetting;
        [InspectorName("强制重新打AB")]
        public bool ForceRebuild = false;
        [InspectorName("更新Manifeist")]
        public bool CopyManifeist = true;
        [InspectorName("更新对比类型")]
        public HotfixBuildType HotfixType = HotfixBuildType.CompareToLast;
        [InspectorName("热更输出根目录")]
        public string HotfixPath = "TestServer/";
    }
}
