using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Orcas.AssetBuilder.Editor;

namespace Orcas.Resources.Editor
{
    public class PackExportSetting : ScriptableObject
    {
        public AssetBundlesBuilderSetting[] ABSettings;
        [InspectorName("导出根目录")]
        public string ExportRoot = "TestServer/";
        [InspectorName("是否重新打AB")]
        public bool ForceRebuild = true;
    }
}