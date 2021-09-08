using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Orcas.Csv;

namespace Orcas.Lua.Core
{
    public class CsvLuaLoader
    {
        public static string LoadConfig(string configName)
        {
            return CsvLoader<string>.CsvFileLoader.LoadCsvFile(configName).text;
        }

        public static string LoadCsv(string csvName, bool isIntKey = true, string content = null)
        {
            if (isIntKey)
            {
                var dic = CsvLoader<int>.ImportAsDic(csvName, content);
                return JsonConvert.SerializeObject(dic);
            }
            else
            {
                var dic = CsvLoader<string>.ImportAsDic(csvName, content);
                return JsonConvert.SerializeObject(dic);
            }
        }

        public static string LoadLanguage(string lanName)
        {
            var list = CsvLoader<string>.ImportAsList(lanName);
            var dic = new Dictionary<string, string>();
            foreach (var item in list)
            {
                dic.Add((string)item["ID"], ((string)item["Value"]).Replace("\\n", "\n"));
            }
            return JsonConvert.SerializeObject(dic);
        }
    }
}

