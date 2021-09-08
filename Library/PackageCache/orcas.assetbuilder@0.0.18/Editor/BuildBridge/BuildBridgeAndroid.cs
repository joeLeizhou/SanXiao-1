//using System.IO;
//using System.Text;
//using Orcas.AssetBuilder.Editor.Interface;
//using Orcas.Core.Tools;
//using UnityEditor;
//using UnityEngine;

//namespace Orcas.AssetBuilder.Editor.BuildBridge
//{
//    public class BuildBridgeAndroid : IBuildBridge
//    {
//        public string GetTargetName()
//        {
//            return "Android";
//        }

//        public string GetPath(AssetBundlesBuilderSetting setting)
//        {
//            if (!setting.ExportRoot.EndsWith("/") && !setting.ExportRoot.EndsWith("\\"))
//            {
//                setting.ExportRoot += '/';
//            }

//            var path = $"{Application.dataPath}/{setting.ExportRoot}Android/";
//            if (setting.VersionExport)
//            {
//                path += setting.Version + "/";
//            }

//            return path;
//        }

//        public AssetBundleManifest BuildAssetBundlesToTempDir(AssetBundlesBuilderSetting setting)
//        {
//            var path = AssetBundleBuilder.TempDir;
//            Utils.ClearOrCreateDirectory(path);
//            return BuildPipeline.BuildAssetBundles(path, setting.Options, BuildTarget.Android);
//        }
//    }
//}