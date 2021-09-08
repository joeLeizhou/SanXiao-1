using System.Globalization;
using System;
using UnityEngine;

namespace Orcas.Csv
{
    public class CsvUtils
    {
        public static float ParseGlobalizationFloat(string str){
            float result;
            if (string.IsNullOrEmpty(str))
                return 0;
            if(!Single.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                result = 0;
                UnityEngine.Debug.LogError("CsvUtils.ParseGlobalizationFloat: Fail to parse float - " + str);   
            }            
            return result;
        }
    }
}