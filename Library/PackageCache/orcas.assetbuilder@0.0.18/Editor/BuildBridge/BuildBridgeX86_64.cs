//using System.IO;
//using System.Text;
//using Orcas.AssetBuilder.Editor.Interface;
//using Orcas.Core.Tools;
//using UnityEditor;
//using UnityEngine;

//namespace Orcas.AssetBuilder.Editor.BuildBridge
//{
//    public class BuildBridgeX86_64 : IBuildBridge
//    {
//        public string GetTargetName()
//        {
//            return "Win64";
//        }

//        public string GetPath(AssetBundlesBuilderSetting setting)
//        {
//            if (!setting.ExportRoot.EndsWith("/") && !setting.ExportRoot.EndsWith("\\"))
//            {
//                setting.ExportRoot += '/';
//            }

//            var path = $"{Application.dataPath}/{setting.ExportRoot}x86_64/";
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
//            return BuildPipeline.BuildAssetBundles(path, setting.Options, BuildTarget.StandaloneWindows64);
//        }
//    }
//}