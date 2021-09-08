using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq;
namespace Orcas.Networking
{
    public class CreateLuaHelper
    {
        public static List<string> CreateLuaFunction(string funcHead, List<string> funcBody)
        {
            List<string> res = new List<string>();
            res.Add(funcHead);

            for (int i = 0; i < funcBody.Count; i++)
            {
                funcBody[i] = "     " + funcBody[i];
            }

            res.AddRange(funcBody);
            res.Add("end");
            return res;
        }

        public static string CreateLuaFunctionHead(string className, string functionName, List<string> arguments = null)
        {
            string funcHead = "function " + className + ":" + functionName + "(";

            if (arguments != null)
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (i != 0) funcHead += ", ";
                    funcHead += arguments[i];
                }
            }
            funcHead += ")";
            return funcHead;
        }
    }
}