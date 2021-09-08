#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Orcas.Networking;
using System.Linq;
using System.IO;

[InitializeOnLoad]
public class AddLuaCustomType : MonoBehaviour
{
    #if USE_TOLUA
    [InitializeOnLoadMethod]
    static void Main()
    {
       // Debug.Log("------------------Add Lua CustomType---------------------------");
        var currentList = CustomSettings.customTypeList;

        ToLuaMenu.BindType[] requiredTypeList = {
        CustomSettings._GT(typeof(HttpServerBase)),
        CustomSettings._GT(typeof(ServerConfig)),
        CustomSettings._GT(typeof(DefaultDecoder)),
        CustomSettings._GT(typeof(UrlInfo)),
        CustomSettings._GT(typeof(APIInfo)),
        CustomSettings._GT(typeof(List<APIInfo>)),
        CustomSettings._GT(typeof(DefaultResponseData)),
        };

        for (int i = 0; i < requiredTypeList.Length; i++)
        {
            var requiredName = requiredTypeList[i].name;
            bool isIncluded = false;
            for (int j = 0; j < currentList.Length; j++)
            {
                if (requiredName == currentList[j].name)
                {
                    isIncluded = true;
                    break;
                }

            }

            if (!isIncluded)
            {
                CustomSettings.customTypeList = CustomSettings.customTypeList.Append(requiredTypeList[i]).ToArray();
            }
        }

        var directory = new DirectoryInfo(CustomSettings.saveDir);
        var wrapCount = directory.GetFiles("*Wrap.cs", SearchOption.AllDirectories).Length;

        if (currentList.Length == CustomSettings.customTypeList.Length)
        {
            Debug.Log("Already added type to list!--------------");
            return;
        }

      //  Debug.Log("customTypeList.Length: " + CustomSettings.customTypeList.Length);
      //  Debug.Log("currentList.Length: " + currentList.Length);
       // Debug.Log("wrapFiles.Length: " + wrapCount);
        var files = directory.GetFiles("*Wrap.cs", SearchOption.AllDirectories);
        var wrapNameList = new List<string>();
        var filesNameList = new List<string>();
        for (int i = 0; i < requiredTypeList.Length; i++)
        {
            wrapNameList.Add(requiredTypeList[i].wrapName + "Wrap.cs");
        }

        for (int i = 0; i < files.Length; i++)
        {
            filesNameList.Add(files[i].Name);
        }


        bool hasGenerated = true;

        for (int i = 0; i < wrapNameList.Count; i++)
        {
            if (!filesNameList.Contains(wrapNameList[i]))
            {
                hasGenerated = false;
                break;
            }
        }

       

        /*
          for (int i = 0; i < currentList.Length; i++)
          {
              wrapNameList.Add(currentList[i].wrapName + "Wrap.cs");
          }
        
    
           for (int i = 0; i < files.Length; i++)
           {
               var fileName = files[i].Name;
               if(!wrapNameList.Contains(fileName))
               {
                   Debug.Log("Extra file Name: " + fileName);
               }
           }*/

        if (hasGenerated)
        {
            Debug.Log("Has Generated added custom type!---------");
            return;
        }

        /*     for (int i = 0; i < wrapNameList.Count; i++)
     {
         Debug.Log("wrapNameList " + i + ": " + wrapNameList[i]);
     }*/

        /* for (int i = 0; i < files.Length; i++)
         {
             Debug.Log(files[i].Name);
             //  Debug.Log("customType Name after add: " + CustomSettings.customTypeList[i].name);
         }*/

        if (wrapCount <= 1)
        {
            Debug.Log("No wrap files!--------------");

        } else if (EditorApplication.isCompiling)
        {
            Debug.Log("-----------------is compling!!!!!!!!-------------------");
        }
        else
        {
            Debug.Log("----------call menu item gen all!!!!!!!!!!!!!!!!------");
            EditorApplication.ExecuteMenuItem("Lua/Generate All");
        }

        //在clear后需要先刷新才能够调用这个方法，不刷新直接点确定的话就会在生成后调用

        //  Debug.Log("customList length after add = " + CustomSettings.customTypeList.Length);
        //CustomSettings.customTypeList.Union(requiredTypeList);
       

    }
    #endif
}
#endif