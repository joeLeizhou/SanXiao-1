using Orcas.Csv;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Orcas.AssetBuilder
{
    public static class CsvFileListUtil
    {
        public static Dictionary<string, CsvFileListInfo> LoadDicAtPath(string path)
        {
            if (!File.Exists(path))
                return null;
            var content = File.ReadAllText(path);
            return LoadDicByContent(content);
        }

        public static Dictionary<string, CsvFileListInfo> LoadDicByContent(string content)
        {
            var dic = CsvLoader<string>.Import<CsvFileListInfo>(content, false);
            var result = new Dictionary<string, CsvFileListInfo>(dic.Count);
            foreach (var item in dic)
            {
                result.Add(item.Key, (CsvFileListInfo)item.Value);
            }
            return result;
        }

        public static List<CsvFileListInfo> LoadAtPath(string path)
        {
            var content = File.ReadAllText(path);
            return LoadByContent(content);
        }

        public static List<CsvFileListInfo> LoadByContent(string content)
        {
            var dic = CsvLoader<string>.Import<CsvFileListInfo>(content, false);
            var list = new List<CsvFileListInfo>();
            foreach (var item in dic)
            {
                list.Add(item.Value as CsvFileListInfo);
            }
            return list;
        }

        public static void Save(List<CsvFileListInfo> fileList, string path)
        {
            var sb = new StringBuilder("");
            sb.AppendLine("ID,BundleName,Size");
            sb.AppendLine("string,string,float");
            sb.AppendLine(",,");
            foreach (var fileInfo in fileList)
            {
                sb.AppendLine($"{fileInfo.ID},{fileInfo.BundleName},{fileInfo.Size:F3}");
            }
            File.WriteAllText(path, sb.ToString(), System.Text.Encoding.UTF8);
        }

        public static void Save(Dictionary<string, CsvFileListInfo> fileList, string path)
        {
            var sb = new StringBuilder("");
            sb.AppendLine("ID,BundleName,Size");
            sb.AppendLine("string,string,float");
            sb.AppendLine(",,");
            foreach (var fileInfo in fileList.Values)
            {
                sb.AppendLine($"{fileInfo.ID},{fileInfo.BundleName},{fileInfo.Size:F3}");
            }
            UnityEngine.Debug.Log("save " + path);
            File.WriteAllText(path, sb.ToString(), System.Text.Encoding.UTF8);
        }

        public static List<string> GetAllID(List<CsvFileListInfo> fileList)
        {
            var result = new List<string>(fileList.Count);
            for (int i = 0; i < fileList.Count; i++)
            {
                result.Add(fileList[i].ID);
            }
            return result;
        }
        public static List<string> GetAllID(Dictionary<string, CsvFileListInfo> dic)
        {
            var result = new List<string>(dic.Count);
            foreach (var item in dic)
            {
                result.Add(item.Key);
            }
            return result;
        }
        public static List<string> GetAllMd5(List<CsvFileListInfo> fileList)
        {
            var result = new List<string>(fileList.Count);
            for (int i = 0; i < fileList.Count; i++)
            {
                result.Add(fileList[i].BundleName);
            }
            return result;
        }
        public static List<string> GetAllMd5(Dictionary<string, CsvFileListInfo> dic)
        {
            var result = new List<string>(dic.Count);
            foreach (var item in dic)
            {
                result.Add(item.Value.BundleName);
            }
            return result;
        }
    }
}