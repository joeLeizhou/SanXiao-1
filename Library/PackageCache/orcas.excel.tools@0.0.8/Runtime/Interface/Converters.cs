using System;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Orcas.Excel
{
    public class ConverterUtils
    {
        public static float ParseGlobalizationFloat(string str){
            if (string.IsNullOrEmpty(str))
                return 0;
            if (float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)) return result;
            result = 0;
            Debug.LogError("CsvUtils.ParseGlobalizationFloat: Fail to parse float - " + str);
            return result;
        }
    }
    
    public class IntConverter : IConverter
    {
        public string Key { get; } = "int";
        public Type KeyType { get; } = typeof(int);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            return int.Parse(str);
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            output.Append(obj);
        }
    }

    public class IntArrConverter : IConverter
    {
        public string Key { get; } = "intArr";
        public Type KeyType { get; } = typeof(int[]);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var pars = str.Split('&');
            var value = new int[pars.Length];
            for (var k = 0; k < pars.Length; k++)
            {
                value[k] = int.Parse(pars[k]);
            }

            return value;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            if (!(obj is int[] intArr)) return;
            for (var x = 0; x < intArr.Length; x++)
            {
                output.Append(intArr[x]);
                if (x < intArr.Length - 1)
                {
                    output.Append("&");
                }
            }
        }
    }

    public class IntArrArrConverter : IConverter
    {
        public string Key { get; } = "intArrArr";
        public Type KeyType { get; } = typeof(int[][]);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var pars = str.Split('&');
            var value = new int[pars.Length][];
            for (var k = 0; k < pars.Length; k++)
            {
                var levels = pars[k].Split('|');
                value[k] = new int[levels.Length];
                for (var w = 0; w < levels.Length; w++)
                {
                    value[k][w] = int.Parse(levels[w]);
                }
            }
            return value;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            Debug.LogWarning("CsvLoader:尝试把二维Int数组转化为字符串.");
        }
    }

    public class BoolConverter : IConverter
    {
        public string Key { get; } = "bool";
        public Type KeyType { get; } = typeof(bool);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            return int.Parse(str) != 0;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            output.Append((bool)obj ? 1 : 0);
        }
    }

    public class BoolArrConverter : IConverter
    {
        public string Key { get; } = "boolArr";
        public Type KeyType { get; } = typeof(bool[]);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var pars = str.Split('&');
            var value = new bool[pars.Length];
            for (var k = 0; k < pars.Length; k++)
            {
                value[k] = int.Parse(pars[k]) != 0;
            }

            return value;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            if (!(obj is bool[] boolArr)) return;
            for (var x = 0; x < boolArr.Length; x++)
            {
                output.Append(boolArr[x] ? 1 : 0);
                if (x < boolArr.Length - 1)
                {
                    output.Append("&");
                }
            }
        }
    }

    public class MaskConverter : IConverter
    {
        public string Key { get; } = "mask";
        public Type KeyType { get; } = typeof(Enum);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0;
            var pars = str.Split('|');
            var value = pars.Aggregate(0, (current, t) => current | int.Parse(t));

            return value;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            Debug.LogWarning("CsvLoader:尝试把mask转化为字符串.");
        }
    }

    public class StringConverter : IConverter
    {
        public string Key { get; } = "string";
        public Type KeyType { get; } = typeof(string);
        public object Covert2Value(string str)
        {
            return str;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            output.Append(obj);
        }
    }

    public class StringArrConverter : IConverter
    {
        public string Key { get; } = "stringArr";
        public Type KeyType { get; } = typeof(string[]);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            var pars = str.Split('&');
            var value = new string[pars.Length];
            for (var k = 0; k < pars.Length; k++)
            {
                value[k] = pars[k];
            }

            return value;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            if (!(obj is string[] stringArr)) return;
            for (var x = 0; x < stringArr.Length; x++)
            {
                output.Append(stringArr[x]);
                if (x < stringArr.Length - 1)
                {
                    output.Append("&");
                }
            }
        }
    }

    public class FloatConverter : IConverter
    {
        public string Key { get; } = "float";
        public Type KeyType { get; } = typeof(float);
        public object Covert2Value(string str)
        {
            if (string.IsNullOrEmpty(str))
                return 0f;
            return ConverterUtils.ParseGlobalizationFloat(str);
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            output.Append(obj);
        }
    }

    public class FloatArrConverter : IConverter
    {
        public string Key { get; } = "floatArr";
        public Type KeyType { get; } = typeof(float[]);
        public object Covert2Value(string str)
        {
            var pars = str.Split('&');
            var value = new float[pars.Length];
            for (var k = 0; k < pars.Length; k++)
            {
                value[k] = ConverterUtils.ParseGlobalizationFloat(pars[k]);
            }

            return value;
        }

        public void Covert2String(object obj, StringBuilder output)
        {
            if (!(obj is float[] floatArr)) return;
            for (var x = 0; x < floatArr.Length; x++)
            {
                output.Append(floatArr[x]);
                if (x < floatArr.Length - 1)
                {
                    output.Append("&");
                }
            }
        }
    }
}