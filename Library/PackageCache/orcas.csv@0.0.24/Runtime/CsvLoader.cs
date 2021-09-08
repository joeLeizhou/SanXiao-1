using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Orcas.Csv
{
    /// <summary>
    /// Csv加载工具类
    /// 要求csv文件名字和数据类名字一样
    /// 前3行数据分别为：变量名、变量类型、说明
    /// 至少需要包含ID
    /// </summary>
    public static class CsvLoader<TKey> where TKey : IConvertible
    {
        private static readonly Dictionary<Type, Dictionary<TKey, ICsvData<TKey>>> CsvMap = new Dictionary<Type, Dictionary<TKey, ICsvData<TKey>>>();

        public static readonly List<IConverter> Converters = new List<IConverter>()
        {
            new IntConverter(),
            new BoolConverter(),
            new FloatConverter(),
            new StringConverter(),
            new FloatArrConverter(),
            new IntArrConverter(),
            new MaskConverter(),
            new StringArrConverter(),
            new BoolArrConverter(),
            new IntArrArrConverter(),
        };
        public static ICsvFileLoader CsvFileLoader { get; set; } = new DefaultCsvResourcesLoader();

        public static void SetPath(string path)
        {
            CsvFileLoader.CsvPath = path;
        }

        public static T GetCsvData<T>(TKey id) where T : ICsvData<TKey>
        {
            if (CsvMap[typeof(T)].ContainsKey(id)) return (T)CsvMap[typeof(T)][id];
            Debug.LogError("Csv " + typeof(T) + " not ContainsKey " + id);
            return default;
        }

        private static List<string> SplitCsvString(string input, char splitChars)
        {
            var ret = new List<string>();
            var index = 0;
            bool flag;
            int i, j;
            var startWithQuote = false;
            var tryEndWithQuote = false;
            var sb = new StringBuilder();
            for (i = 0; i < input.Length; i++)
            {
                if (index == i)
                {
                    if (input[i] == '"')
                    {
                        startWithQuote = true;
                        continue;
                    }
                }

                if (startWithQuote)
                {
                    if (tryEndWithQuote)
                    {
                        if (input[i] == '"')
                        {
                            sb.Append('"');
                            tryEndWithQuote = false;
                            continue;
                        }
                    }

                    if (!tryEndWithQuote)
                    {
                        if (input[i] == '"')
                        {
                            tryEndWithQuote = true;
                            if (i < input.Length - 1) continue;
                        }
                    }

                    flag = true;
                    if (i == input.Length - 1 || (tryEndWithQuote && input[i] == splitChars))
                    {
                        flag = false;
                    }

                    if (flag)
                    {
                        sb.Append(input[i]);
                        continue;
                    }

                    if (i == input.Length - 1 && tryEndWithQuote == false)
                    {
                        sb.Append(input[i]);
                    }

                    startWithQuote = tryEndWithQuote = false;
                    ret.Add(sb.ToString());
                    sb.Clear();
                    index = i + 1;
                }
                else
                {
                    if (input[i] == splitChars)
                    {
                        ret.Add(input.Substring(index, i - index));
                        index = i + 1;
                    }
                    else if (i == input.Length - 1)
                    {
                        ret.Add(input.Substring(index, i - index + 1));
                        index = i + 1;
                    }
                }
            }

            return ret;
        }

        private static List<string> SplitCsvLines(string input, char splitChars)
        {
            var state = 1;
            var index = 0;
            var lines = new List<string>();
            for (var i = 0; i < input.Length; i++)
            {
                if (i == input.Length - 1)
                {
                    switch (input[i])
                    {
                        case '\r':
                        case '\n':
                            lines.Add(input.Substring(index, i - index));
                            break;
                        default:
                            lines.Add(input.Substring(index, i - index + 1));
                            break;
                    }
                }
                else
                {
                    switch (state)
                    {
                        case 0:
                            if (input[i] == splitChars)
                            {
                                state = 1;
                            }
                            else
                            {
                                switch (input[i])
                                {
                                    case '\r':
                                    case '\n':
                                        lines.Add(input.Substring(index, i - index));
                                        index = i + 1;
                                        state = 1;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            break;
                        case 1:
                            switch (input[i])
                            {
                                case '"':
                                    state = 2;
                                    break;
                                case '\r':
                                case '\n':
                                    lines.Add(input.Substring(index, i - index));
                                    index = i + 1;
                                    break;
                                default:
                                    state = 0;
                                    break;
                            }

                            break;
                        case 2:
                            if (input[i] == '"')
                            {
                                state = 3;
                            }

                            break;
                        case 3:
                            if (input[i] == splitChars)
                            {
                                state = 0;
                            }
                            else
                            {
                                switch (input[i])
                                {
                                    case '\r':
                                    case '\n':
                                        lines.Add(input.Substring(index, i - index));
                                        index = i + 1;
                                        state = 0;
                                        break;
                                    default:
                                        state = 2;
                                        break;
                                }
                            }
                            break;
                    }
                }
            }

            var ret = new List<string>();
            for (var i = 0; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) == false)
                {
                    ret.Add(lines[i]);
                }
            }

            return ret;
        }

        public static Dictionary<TKey, ICsvData<TKey>> Import<T>(string text = null, bool save = true) where T : ICsvData<TKey>, new()
        {
            var type = typeof(T);
            if (save)
            {
                if (CsvMap.ContainsKey(type) == true)
                {
                    if (string.IsNullOrEmpty(text) == true)
                        return CsvMap[type];
                    else
                        CsvMap.Remove(type);
                }
            }
            if (text == null)
            {
                TextAsset csv = null;
                csv = CsvFileLoader.LoadCsvFile(type.ToString());
                text = csv.text;
            }
            var separator = text[2] == ';' ? ';' : ',';
            var lines = SplitCsvLines(text, ',');
#if UNITY_EDITOR
            Debug.Log("csv:" + type.ToString() + ";lines:" + lines.Count);
#endif
            if (lines.Count < 3)
            {
                return null;
            }
            var table = new Dictionary<TKey, ICsvData<TKey>>();
            if (save)
            {
                CsvMap.Add(type, table);
            }
            var varName = SplitCsvString(lines[0], separator);
            if (varName[0][0] == 0xFEFF)
                varName[0] = varName[0].Substring(1);
            var varType = SplitCsvString(lines[1], separator);
            var props = type.GetProperties();
            var index = new int[varName.Count];
            for (var i = 0; i < index.Length; i++)
            {
                index[i] = -1;
            }
            for (var i = 0; i < props.Length; i++)
            {
                for (var j = 0; j < varName.Count; j++)
                {
                    if (index[j] == -1)
                    {
                        if (props[i].Name == varName[j])
                        {
                            index[j] = i;
                        }
                    }
                }
            }
            for (var i = 3; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                var col = SplitCsvString(lines[i], separator);

                var data = new T();
                for (var j = 0; j < col.Count; j++)
                {
                    if (index[j] == -1 || string.IsNullOrEmpty(col[j])) continue;
                    var prop = props[index[j]];
                    try
                    {
                        var converter = Converters.Find((a) => a.Key == varType[j]);
                        prop.SetValue(data, converter.Covert2Value(col[j]));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Csv] Error Format: " + " row " + i + "col " + j + ":" + varType[j] + "/" + col[j] + e);
                    }
                }
                if (table.ContainsKey(data.ID) == false)
                {
                    table.Add(data.ID, data);
                }
            }
            return table;
        }
        public static List<Dictionary<string, object>> ImportAsList(string fileName, string text = null)
        {
            if (text == null)
            {
                TextAsset csv = null;
                csv = CsvFileLoader.LoadCsvFile(fileName);
                text = csv.text;
            }
            var separator = text[2] == ';' ? ';' : ',';
            var lines = SplitCsvLines(text, separator);
#if UNITY_EDITOR
            Debug.Log("csv:" + fileName + ";lines:" + lines.Count);
#endif
            if (lines.Count < 3)
            {
                return null;
            }
            var list = new List<Dictionary<string, object>>(lines.Count - 3);
            var varName = SplitCsvString(lines[0], separator);
            if (varName[0][0] == 0xFEFF)
                varName[0] = varName[0].Substring(1);
            var varType = SplitCsvString(lines[1], separator);
            for (var i = 3; i < lines.Count; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue;
                var col = SplitCsvString(lines[i], separator);

                var data = new Dictionary<string, object>();
                for (var j = 0; j < col.Count; j++)
                {
                    try
                    {
                        var converter = Converters.Find((a) => a.Key == varType[j]);
                        data.Add(varName[j], converter.Covert2Value(col[j]));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("[Csv] Error Format: file" + fileName + " row " + i + "col " + j + ":" + varType[j] + "/" + col[j] + e);
                    }
                }
                list.Add(data);
            }
            return list;
        }
        public static Dictionary<TKey, Dictionary<string, object>> ImportAsDic(string fileName, string text = null)
        {
            var list = ImportAsList(fileName, text);
            var table = new Dictionary<TKey, Dictionary<string, object>>(list.Count);
            foreach (var item in list)
            {
                table[(TKey)item["ID"]] = item;
            }
            return table;
        }
        public static void Clear()
        {
            CsvMap.Clear();
        }

        public static int GetCount<T>()
        {
            var type = typeof(T);
            return CsvMap.ContainsKey(type) == false ? 0 : CsvMap[type].Count;
        }

        public static void SaveCsv<T>()
        {
            var type = typeof(T);
            if (CsvMap.ContainsKey(type) == false)
            {
                return;
            }
            TextAsset csv = null;
            csv = CsvFileLoader.LoadCsvFile(type.ToString());
            var sb = new System.Text.StringBuilder();
            var tLines = csv.text.Split('\r', '\n');
            var lines = tLines.Where(t => string.IsNullOrEmpty(t) == false).ToArray();

            var table = CsvMap[type];
            var varName = lines[0].Split(',', '\r');
            var varType = lines[1].Split(',', '\r');
            var props = type.GetProperties();
            var index = new int[varName.Length];
            for (var i = 0; i < index.Length; i++)
            {
                index[i] = -1;
            }
            for (var i = 0; i < props.Length; i++)
            {
                for (var j = 0; j < varName.Length; j++)
                {
                    if (index[j] != -1) continue;
                    if (props[i].Name == varName[j])
                    {
                        index[j] = i;
                    }
                }
            }

            sb.Append(lines[0]); sb.Append("\n");
            sb.Append(lines[1]); sb.Append("\n");
            sb.Append(lines[2]); sb.Append("\n");
            foreach (var kv in table) kv.Value.Saved = false;
            for (var i = 3; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i]) != false) continue;
                var col = lines[i].Split(',', '\r');
                if (col.Length < 2) { sb.Append(lines[i]); sb.Append("\n"); continue; }
                TKey id = (TKey)Convert.ChangeType(col[0], typeof(TKey));
                if (table.ContainsKey(id) == false) { sb.Append(lines[i]); sb.Append("\n"); continue; }
                var data = table[id];
                for (var j = 0; j < col.Length; j++)
                {
                    if (index[j] != -1)
                    {
                        var prop = props[index[j]];
                        try
                        {
                            var converter = Converters.Find((a) => a.Key == varType[j]);
                            converter.Covert2String(prop.GetValue(data), sb);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        sb.Append(col[j]);
                    }
                    if (j < col.Length - 1)
                    {
                        sb.Append(',');
                    }
                }
                sb.Append("\r\n");
                data.Saved = true;
            }
            foreach (var kv in table.Where(kv => kv.Value.Saved == false))
            {
                for (var j = 0; j < varName.Length; j++)
                {
                    if (index[j] != -1)
                    {
                        var prop = props[index[j]];
                        try
                        {
                            var converter = Converters.Find((a) => a.Key == varType[j]);
                            converter.Covert2String(prop.GetValue(kv.Value), sb);
                        }
                        catch (Exception) { }
                    }
                    if (j < varName.Length - 1)
                    {
                        sb.Append(',');
                    }
                }
                sb.Append("\r\n");
            }
#if UNITY_EDITOR
            File.WriteAllText(Path.GetFullPath("Assets/Resources/" + CsvFileLoader.CsvPath + type + ".csv"), sb.ToString(), System.Text.Encoding.UTF8);
#endif
        }

        public static void SaveList<T>(List<T> datas, string path) where T : ICsvData<TKey>
        {
            var type = typeof(T);
            var props = type.GetProperties().ToList();
            var saveIndex = props.FindIndex(p => p.Name == "Saved");
            props.RemoveAt(saveIndex);
            var sb = new System.Text.StringBuilder();
            var converters = new List<IConverter>(props.Count);
            for (int i = 0; i < props.Count; i++)
            {
                sb.Append(props[i].Name);
                if (i < props.Count - 1) sb.Append(",");
                converters.Add(Converters.Find(x => x.KeyType == props[i].PropertyType));
            }
            sb.AppendLine();
            for (int i = 0; i < props.Count; i++)
            {
                sb.Append(converters[i].Key);
                if (i < props.Count - 1) sb.Append(",");
            }
            sb.AppendLine();
            for (int i = 0; i < props.Count; i++)
            {
                if (i < props.Count - 1) sb.Append(",");
            }
            sb.AppendLine();
            Debug.Log(type.Name + "header \n" + sb.ToString());
            for (int i = 0; i < datas.Count; i++)
            {
                for (int j = 0; j < props.Count; j++)
                {
                    converters[j].Covert2String(props[j].GetValue(datas[i]), sb);
                    if (j < props.Count - 1) sb.Append(",");
                }
                sb.AppendLine();
            }
#if UNITY_EDITOR
            string fullPath = Path.GetFullPath(path) + type.Name + ".csv";
            File.WriteAllText(fullPath, sb.ToString(), System.Text.Encoding.UTF8);
#endif
        }

        public static Dictionary<TKey, ICsvData<TKey>> GetAll<T>()
        {
            var type = typeof(T);
            return CsvMap[type];
        }

        public static List<T> GetAllList<T>(Func<T, int> comparer) where T : ICsvData<TKey>
        {
            var datas = CsvMap[typeof(T)];
            var list = new List<T>(datas.Count);
            foreach (var item in datas.OrderBy(item => comparer((T)(item.Value))))
            {
                list.Add((T)item.Value);
            }
            return list;
        }

        public static void SortAndSave<T>(Dictionary<TKey, ICsvData<TKey>> datas, Func<T, int> comparer, string path) where T : ICsvData<TKey>
        {
            var list = new List<T>(datas.Count);
            foreach (var item in datas.OrderBy(item => comparer((T)(item.Value))))
            {
                list.Add((T)item.Value);
            }
            SaveList(list, path);
        }
        
    }
}
