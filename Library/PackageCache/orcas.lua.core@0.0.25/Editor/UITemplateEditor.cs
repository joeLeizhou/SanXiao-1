using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace Orcas.Lua.Core.Editor
{
    
    public class UITemplateEditor : EditorWindow
    {
        private static Dictionary<string, string> fileAndPath;
        private static GameObject selectObject;
        private static List<GameObject> panels;

        private static bool NeedUpdate = false;
        private static bool NeedSecondUpdate = false;
        private static int Depath = 0;
        private static string EnumName = "";
        private static string configStr = "";
        private static string prefabFolder = "";
        /// <summary>
        /// 可搜寻的组件名称
        /// </summary>
        private static List<string> ComponentStrs = new List<string>() { "Con_", "Btn_", "InputTxt_", "Txt_", "Drp_", "Sld_", "Scr_", "Tog_", "SView_", "Img_", "RImg_", "Obj_" };
        /// <summary>
        /// 要添加事件的组件名称
        /// </summary>
        private static List<string> EventComponentStrs = new List<string>() { "Btn_", "InputTxt_", "Tog_" };

        //层级用
        private static int selGridInt = 1;
        private static string[] selStrings = new string[] { "LAYER_BOTTOM", "LAYER_NORMAL", "LAYER_MIDDLE", "LAYER_TOP", "LAYER_HIGHEST" };

        [MenuItem("GameObject/UI脚本/生成Lua Panel", false, 10)]
        public static void ShowWindow()
        {
            selectObject = Selection.activeGameObject;
            if (selectObject == null) return;

            panels = new List<GameObject>();
            foreach (Transform child in selectObject.GetComponentsInChildren<Transform>(true))
            {
                if (child.name.Contains("Panel"))
                {
                    panels.Add(child.gameObject);
                }
            }

            EditorWindow.GetWindow(typeof(UITemplateEditor));
        }

        void OnGUI()
        {
            if (selectObject == null) return;
            GameObject scriptTargetObj = null;

            if (panels.Count == 0) return;

            GUI.Label(new Rect(10, 10, 100, 20), "脚本文件名");
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].activeSelf)
                {
                    scriptTargetObj = panels[i];
                    panels[i].name = GUI.TextField(new Rect(80, 10, 100, 20), panels[i].name);
                    break;
                }
            }
            if (scriptTargetObj == null)
            {
                scriptTargetObj = panels[0];
            }
            if (scriptTargetObj == null) return;

            GUI.Label(new Rect(10, 40, 100, 20), "检测到的面板列表");
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].SetActive(GUI.Toggle(new Rect(10, 60 + i * 20, 120, 20), panels[i].activeSelf, panels[i].name));
            }

            EnumName = scriptTargetObj.name;
            string EnumNameCopy = "";
            for (int i = 0; i < EnumName.Length; i++)
            {
                if (i != 0 && i != 1 && EnumName[i] < 'Z' && EnumName[i] > 'A')
                {
                    EnumNameCopy += "_" + EnumName[i];
                }
                else
                {
                    EnumNameCopy += EnumName[i];
                }
            }
            EnumNameCopy = EnumNameCopy.ToUpper();
            EnumName = EnumNameCopy;
            GUI.Label(new Rect(250, 10, 100, 20), "枚举名");
            GUI.TextField(new Rect(320, 10, 100, 20), EnumName);

            GUI.Label(new Rect(550, 10, 100, 20), "prefab目录");
            prefabFolder = GUI.TextField(new Rect(620, 10, 100, 20), prefabFolder);

            //层级处理
            selGridInt = GUI.SelectionGrid(new Rect(130, 100, 100, 100), selGridInt, selStrings, 1);

            configStr = @"PrefabPath = 'Assets/External/Prefabs/Prefab_UI/" + (prefabFolder.Length > 0 ? prefabFolder + "/" : "") + scriptTargetObj.name + @"',
	NeedUpdate = " + (NeedUpdate = GUI.Toggle(new Rect(130, 60, 120, 20), NeedUpdate, "更新")).ToString().ToLower() + @",
    NeedSecondUpdate = " + (NeedSecondUpdate = GUI.Toggle(new Rect(130, 80, 120, 20), NeedSecondUpdate, "秒更")).ToString().ToLower() + @",
    Depth = UIDepthEnum." + selStrings[selGridInt];

            configStr = configStr.Replace('\'', '\"');
            configStr = GUI.TextArea(new Rect(250, 40, 350, 120), configStr);

            if (GUI.Button(new Rect(250, 180, 100, 20), "复制"))
            {
                TextEditor te = new TextEditor();
                te.text = configStr;
                te.OnFocus();
                te.Copy();
            }

            if (GUI.Button(new Rect(10, 180, 100, 20), "生成脚本"))
            {
                CreatLuaScript(scriptTargetObj, scriptTargetObj.name, "PanelLuaTemplate", configStr);
            }
        }

        [MenuItem("GameObject/UI脚本/生成Lua Component", false, 10)]
        public static void CreatLuaComponet()
        {
            CreatLuaScript(Selection.activeGameObject, Selection.activeGameObject.name, "ComponetLuaTemplate");
        }


        static void CreatLuaScript(GameObject gameObj, string panelName, string templateName, string tarConfStr = "")
        {
            if (fileAndPath == null) fileAndPath = new Dictionary<string, string>();
            fileAndPath.Clear();

            string filePath = "", fileName = panelName;

            getFilePathAndName(ref filePath, ref fileName);

            if (gameObj == null)
            {
                Debug.LogError("UI对象为空");
                return;
            }

            string templateContent = getTemplateString(templateName);

            if (tarConfStr.Length <= 1)
            {
                configStr = @"PrefabPath = 'Assets/External/Prefabs/Prefab_UI/" + panelName + @"',
	    NeedUpdate = false" + @",
	    NeedSecondUpdate = false" + @",
	    Depth = UIDepthEnum.LAYER_BOTTOM";
            }
            else
            {
                configStr = tarConfStr;
            }

            configStr = configStr.Replace('\'', '\"');

            EnumName = panelName;
            string EnumNameCopy = "";
            for (int i = 0; i < EnumName.Length; i++)
            {
                if (i != 0 && i != 1 && EnumName[i] < 'Z' && EnumName[i] > 'A')
                {
                    EnumNameCopy += "_" + EnumName[i];
                }
                else
                {
                    EnumNameCopy += EnumName[i];
                }
            }
            EnumNameCopy = EnumNameCopy.ToUpper();
            EnumName = EnumNameCopy;

            templateContent = templateContent.Replace("#INIT_DATA#", configStr);
            templateContent = templateContent.Replace("#SELF_ENUM#", EnumName);
            templateContent = templateContent.Replace("#NAME#", fileName);
            templateContent = templateContent.Replace("#InitComponent#", getComponentString(fileName, gameObj));
            templateContent = templateContent.Replace("#AddEventListener#", getEventListenerString(fileName));
            templateContent = templateContent.Replace("#RemoveEventListener#", getRemoveEventListenerString(fileName));

            FileStream fs = new FileStream(filePath, FileMode.Create);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);  
            StreamWriter sw = new StreamWriter(fs, new System.Text.UTF8Encoding(false));
            Debug.Log("fzy str" + templateContent);
            sw.Write(templateContent);
            sw.Flush();
            sw.Close();
            fs.Close();

            AssetDatabase.Refresh(ImportAssetOptions.Default);
        }

        private static void getFilePathAndName(ref string _filePath, ref string _fileName)
        {
            _filePath = EditorUtility.SaveFilePanel("保存脚本", "Assets/Scripts/Lua/", _fileName, "lua");
            FileInfo fileInfo = new FileInfo(_filePath);
            _fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
        }

        public static string getTemplateString(string templateName)
        {
            string templatePath = "Assets/Scripts/Lua/OrcasLua/Template/" + templateName + ".lua";
            if (File.Exists(templatePath))
            {
                StreamReader sr = File.OpenText(templatePath);
                string str = sr.ReadToEnd();
                sr.Close();
                return str;
            }
            return "";
        }

        private static string getComponentString(string fileName, GameObject obj)
        {
            string str = "";
            foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            {
                for (int i = 0; i < ComponentStrs.Count; i++)
                {
                    if (child.name.Contains(ComponentStrs[i]))
                    {
                        fileAndPath[child.name] = getChildPath(obj.transform, child);
                        if (child.name.StartsWith("Btn_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Button\")\r";
                        }
                        else if (child.name.StartsWith("Txt_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Text\")\r";
                        }
                        else if (child.name.StartsWith("InputTxt_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"InputField\")\r";
                        }
                        else if (child.name.StartsWith("Drp_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Dropdown\")\r";
                        }
                        else if (child.name.StartsWith("Sld_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Slider\")\r";
                        }
                        else if (child.name.StartsWith("Scr_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Scrollbar\")\r";
                        }
                        else if (child.name.StartsWith("Tog_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Toggle\")\r";
                        }
                        else if (child.name.StartsWith("SView_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"ScrollRect\")\r";
                        }
                        else if (child.name.StartsWith("RImg_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"RawImage\")\r";
                        }
                        else if (child.name.StartsWith("Img_"))
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject):GetComponent(\"Image\")\r";
                        }
                        else
                        {
                            str += "    private." + child.name + " = " + "ResourceLoader.GetGameObjectInParentByPath(\"" + fileAndPath[child.name] + "\", private.gameObject)\r";
                        }
                        break;
                    }
                }
            }

            return str;
        }
        private static string getChildPath(Transform root, Transform child)
        {
            string str = child.name;
            while (child.parent != root && child.parent != null)
            {
                str = child.parent.name + "/" + str;
                child = child.parent;
            }
            return str;
        }

        private static string getEventListenerString(string fileName)
        {
            string str = "";
            foreach (string key in fileAndPath.Keys)
            {
                for (int i = 0; i < EventComponentStrs.Count; i++)
                {
                    if (key.Contains(EventComponentStrs[i]))
                    {
                        if (EventComponentStrs[i] == "Tog_")
                        {
                            str += "    UIEventManager.Get(private." + key + ".gameObject):ToggleOnValueChanged(private.[定义事件（bool）])\r";
                        }
                        else if (EventComponentStrs[i] == "InputTxt_")
                        {
                            //str += "    UIEventManager.Get(" + fileName + "." + key + ".gameObject):onClick(" + fileName + ".[请修改])\r";
                        }
                        else
                        {
                            str += "    UIEventManager.Get(private." + key + ".gameObject):onClick(private.OnButtonClick)\r";
                            break;
                        }

                    }
                }
            }

            return str;
        }

        private static string getRemoveEventListenerString(string fileName)
        {
            string str = "";
            foreach (string key in fileAndPath.Keys)
            {
                for (int i = 0; i < EventComponentStrs.Count; i++)
                {
                    if (key.Contains(EventComponentStrs[i]))
                    {
                        str += "    UIEventManager.Get(private." + key + ".gameObject):removeAllEvent()\r";
                        break;
                    }
                }
            }

            return str;
        }
    }
}
