using Orcas.Core.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Orcas.AssetBuilder.Editor
{
    public partial class AssetBundleBuilder
    {
        private static void DfsPath(string dir, Action<string> callBack)
        {
            dir = Path.GetFullPath(dir);
            var allFiles = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            for (int i = 0; i < allFiles.Length; i++)
            {
                if (!allFiles[i].EndsWith(".meta", StringComparison.Ordinal))
                    callBack(allFiles[i]);
            }
        }

        public static void ClearAssetBundlesName()
        {
            var abNames = AssetDatabase.GetAllAssetBundleNames();
            Debug.Log("clear ab names count:" + abNames.Length);
            for (int i = 0; i < abNames.Length; i++)
            {
                var paths = AssetDatabase.GetAssetPathsFromAssetBundle(abNames[i]);
                foreach (var path in paths)
                {
                    var importer = AssetImporter.GetAtPath(path);
                    importer.assetBundleName = "";
                    importer = AssetImporter.GetAtPath(Path.GetDirectoryName(path));
                    if (importer != null) importer.assetBundleName = "";
                }
                EditorUtility.DisplayProgressBar("Clear AssetBundle Names", abNames[i], (float)i / abNames.Length);
            }
        }

#if UNITY_ANDROID
        public static readonly string OSDir = "Android";
#elif UNITY_IOS
        public static readonly string OSDir = "iOS";
#elif UNITY_EDITOR || UNITY_STANDALONE
        public static readonly string OSDir = "StandAlone";
#endif

        public static string GetExportOSRoot(AssetBundlesBuilderSetting setting)
        {

            var path = $"{Path.GetFullPath(setting.ExportRoot)}{OSDir}/";

            if (setting.VersionExport)
            {
                path += setting.Version + "/";
            }
            //if (setting.PackID > 0)
            //{
            //    path += setting.PackID.ToString() + "/";
            //}

            return path;
        }

        public static string GetPackManifest(int packID)
        {
            return packID == 0 ? "Manifest" : "Manifest" + packID;
        }
        /// <summary>
        /// 拷贝目录
        /// https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                EditorUtility.DisplayProgressBar("DirectoryCopy", tempPath, 0.1f);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}