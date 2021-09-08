using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Orcas.Csv;
using System.Text;

namespace Orcas.Csv.Editor
{
    public class LanguageEditor : EditorWindow
    {
        [MenuItem("Orcas/工具/导入语言表")]
        static void init()
        {
            var window = GetWindow<LanguageEditor>();
        }

        private static string sourceDir = Application.dataPath;

        private static string sourcePath = Path.GetFullPath("misc/Config/CsvLanguage.csv");

        private static string destDir = Application.dataPath + "/Resources/External/Csv/";
        private void OnGUI()
        {
            if (GUILayout.Button("选择语言源文件"))
                sourcePath = EditorUtility.OpenFilePanel("选择语言源文件", sourceDir, "csv");
            sourcePath = EditorGUILayout.TextField("语言源文件", sourcePath);
            GUILayout.Space(20);

            if (GUILayout.Button("选择输出目录"))
                destDir = EditorUtility.OpenFolderPanel("选择输出目录", destDir, "");
            destDir = EditorGUILayout.TextField("输出目录", destDir);
            GUILayout.Space(20);

            if (GUILayout.Button("导出"))
            {
                if (File.Exists(sourcePath) == false)
                {
                    EditorUtility.DisplayDialog("错误", "源文件不存在", "OK");
                }
                else if (Directory.Exists(destDir) == false)
                {
                    EditorUtility.DisplayDialog("错误", "输出目录不存在", "OK");
                }
                else
                {
                    Export();
                }
            }
        }

        private void Export()
        {
            var content = File.ReadAllText(sourcePath);
            var list = CsvLoader<string>.ImportAsList("", content);
            var keys = list[0].Keys.ToList();
            var idKey = "ID";
            keys.Remove(idKey);
            var lanDic = new Dictionary<string, List<KeyValuePair<string, string>>>(keys.Count);
            foreach (var key in keys)
            {
                lanDic.Add(key, new List<KeyValuePair<string,string>>());
            }

            foreach (var item in list)
            {
                foreach (var key in keys)
                {
                    var lanList = lanDic[key];
                    lanList.Add(new KeyValuePair<string, string>((string)item[idKey], (string)item[key]));
                }
            }
            var sb = new StringBuilder();
            foreach (var key in keys)
            {
                sb.Clear();
                sb.AppendLine("ID;Value");
                sb.AppendLine("string;string");
                sb.AppendLine(";");
                var lanList = lanDic[key];
                for (int i = 0; i < lanList.Count; i++)
                {
                    var pair = lanList[i];
                    sb.AppendLine($"{pair.Key};{pair.Value}");
                }
                var savePath = Path.GetFullPath(destDir) + "/Csv" + key + ".csv";
                File.WriteAllText(savePath, sb.ToString(), System.Text.Encoding.UTF8);
            }
            EditorUtility.DisplayDialog("完成", "导入语言完成\n\n导入: " + string.Join("; ", keys), "OK");
        }
    }
}