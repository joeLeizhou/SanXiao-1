using UnityEditor;
using UnityEngine;
using Orcas.Networking;
using System.IO;

[CustomEditor(typeof(ServerConfig)), CanEditMultipleObjects]
public class ServerConfigInspector : Editor
{
    private bool _visible = true;
    private bool _apiListVisible = true;
    private ServerConfig _serverConfig;

    private void OnEnable()
    {
       

    }

    public override void OnInspectorGUI()
    {
        var protocolType_Prop = serializedObject.FindProperty("_protocolType");
        var serverName_Prop = serializedObject.FindProperty("_serverName");

        _serverConfig = (ServerConfig)target;

        serializedObject.Update();
    
        EditorGUILayout.PropertyField(serverName_Prop);
        EditorGUILayout.PropertyField(protocolType_Prop);

        ServerConfig.ProtocolType currentProtocolType = (ServerConfig.ProtocolType)protocolType_Prop.enumValueIndex;
        ShowDecodeMethod();

        if (currentProtocolType == ServerConfig.ProtocolType.Http)
        { 
            ShowUrlList("_urlList", ref _visible);
            ApiListIterator("_apiList", ref _apiListVisible);
        }
        else
        {
            SerializedProperty url_Prop = serializedObject.FindProperty("_url");
            SerializedProperty port_Prop = serializedObject.FindProperty("_port");
            SerializedProperty encrypt_Prop = serializedObject.FindProperty("_encryptionMethod");
            EditorGUILayout.PropertyField(url_Prop, new GUIContent("Url"));
            EditorGUILayout.PropertyField(port_Prop, new GUIContent("Port"));
            EditorGUILayout.PropertyField(encrypt_Prop, new GUIContent("Encrypt Method"));

        }

        GUILayout.Space(40);
        GUI.skin.button.fontSize = 20;
        serializedObject.ApplyModifiedProperties();

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("保存", GUILayout.Height(40), GUILayout.Width(100)))
            {

                ServersMaintainer.DeleteServer(_serverConfig.name);
               // _serverConfig.name = _serverConfig.GetServerName();
                ServersMaintainer.AddServer(_serverConfig);
                if (_serverConfig.name != _serverConfig.GetServerName())
                CreateNewNameServerConfig();
            }

            GUILayout.Space(50);

            if (GUILayout.Button("删除", GUILayout.Height(40), GUILayout.Width(100)))
            {
                ServersMaintainer.DeleteServer(_serverConfig.name);
                DeleteServerConfigFiles();
            }
        }
        GUILayout.EndHorizontal();
    }

    public void ShowUrlList(string propertyName, ref bool visible)
    {
        SerializedProperty listProperty = serializedObject.FindProperty(propertyName);

        if (listProperty.arraySize > 0)
        {
            SerializedProperty defaultUrlIndex_Prop = serializedObject.FindProperty("_defaultUrlIndex");
            EditorGUILayout.PropertyField(defaultUrlIndex_Prop);
        }

        if (listProperty.arraySize <= _serverConfig.GetDefaultUrlIndex())
        {
            _serverConfig.ResetDefaultUrlIndex();
        }

        _visible = EditorGUILayout.Foldout(visible, "URL List");

        GUI.skin.button.fontSize = 16;

        if (visible)
        {
            DrawSeparator(Color.red);
            EditorGUI.indentLevel++;
            for (int i = listProperty.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                GUI.skin.label.fontSize = 16;
                GUI.skin.label.fontStyle = FontStyle.Bold;

                elementProperty.Next(true);
                var url = elementProperty.stringValue;   //url
                var titleName = i.ToString() + ": " + ((url == "") ? "New URL" : url);

                GUILayout.BeginHorizontal();
                {
                    _serverConfig.GetUrlInfo(i)._visible = EditorGUILayout.Foldout(_serverConfig.GetUrlInfo(i)._visible, titleName);
                    GUILayout.Space(30);
                    GUI.skin.button.fontSize = 16;
                    GUI.skin.button.fontStyle = FontStyle.Bold;
                    if (GUILayout.Button("✘", GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        _serverConfig.deleteUrlItem(i);
                        continue;
                    }
                }
                GUILayout.EndHorizontal();

                if (!_serverConfig.GetUrlInfo(i)._visible) continue;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(elementProperty);

                elementProperty.Next(false);
                EditorGUILayout.PropertyField(elementProperty);
                var x2 = (UrlInfo.UrlType)elementProperty.enumValueIndex;  //type

                elementProperty.Next(false);
                if (x2 == UrlInfo.UrlType.Custom)
                {
                    EditorGUILayout.PropertyField(elementProperty);
                }

                elementProperty.Next(false);
                EditorGUILayout.PropertyField(elementProperty);
                var x4 = elementProperty.intValue;  //port

                GUI.skin.button.fontSize = 14;

                EditorGUI.indentLevel--;

                DrawSeparator(Color.grey);
            }

            EditorGUI.indentLevel--;
            GUILayout.Space(10);

            if (GUILayout.Button("✚", GUILayout.Height(30), GUILayout.Width(80)))
            {
                _serverConfig.AddUrlItem();
            }

            GUILayout.Space(10);
            DrawSeparator(Color.red);
        }

    }

    public void ApiListIterator(string propertyPath, ref bool visible)
    {
        SerializedProperty listProperty = serializedObject.FindProperty(propertyPath);
        visible = EditorGUILayout.Foldout(visible, "API List");
        if (visible)
        {
            DrawSeparator(Color.yellow);
            EditorGUI.indentLevel++;
            //Debug.Log("api-arraySize: " + listProperty.arraySize);
            for (int i = listProperty.arraySize - 1; i >= 0; i--)
            {

                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                elementProperty.Next(true);

                var path = elementProperty.stringValue;   //path
                var apiName = GetNameFromPath(path);
                var titleName = (apiName == "") ? "New API" : apiName;

                GUILayout.BeginHorizontal();
                {
                    _serverConfig.GetApiList()[i]._visible = EditorGUILayout.Foldout(_serverConfig.GetApiList()[i]._visible, titleName);

                    GUILayout.Space(30);
                    GUI.skin.button.fontSize = 16;
                    GUI.skin.button.fontStyle = FontStyle.Bold;
                    if (GUILayout.Button("✘", GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        _serverConfig.DeleteApiItem(i);
                        continue;
                    }
                }
                GUILayout.EndHorizontal();

                if (!_serverConfig.GetApiList()[i]._visible) continue;

                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(elementProperty);

                elementProperty.Next(false);
                EditorGUILayout.PropertyField(elementProperty);
                var x2 = (UrlInfo.UrlType)elementProperty.enumValueIndex;  //method

                elementProperty.Next(false);
                EditorGUILayout.PropertyField(elementProperty); //format

                elementProperty.Next(true);

                ParameterListIterator(elementProperty, i, ref _serverConfig.GetApiList()[i]._parameterListVisible);

                GUILayout.Space(10);

                EditorGUI.indentLevel--;
                DrawSeparator(Color.grey);
            }

            GUILayout.Space(20);

            GUI.skin.button.fontStyle = FontStyle.Bold;
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("✚", GUILayout.Height(30), GUILayout.Width(80)))
                {
                    _serverConfig.AddApiItem();
                }
                GUILayout.Space(150);

               // if (GUILayout.Button("保存", GUILayout.Height(30), GUILayout.Width(80)))
                //{
                   
                //}
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            DrawSeparator(Color.yellow);
            GUILayout.Space(30);

        }
        EditorGUI.indentLevel--;
    }

    public void ParameterListIterator(SerializedProperty listProperty, int apiIndex, ref bool visible)
    {
        visible = EditorGUILayout.Foldout(visible, "Parameter List");
        var apiList = _serverConfig.GetApiList();
        var apiInfo = apiList[apiIndex];
        var parameterList = apiInfo.GetParameterList();
        if (visible)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < listProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                GUILayout.BeginHorizontal();
                {
                    elementProperty.Next(true);
                    EditorGUILayout.PropertyField(elementProperty, new GUIContent(""), GUILayout.Width(150));

                    GUILayout.Space(30);

                    elementProperty.Next(false);
                    EditorGUILayout.PropertyField(elementProperty, new GUIContent(""), GUILayout.Width(200));

                    GUILayout.Space(50);

                    GUI.skin.button.fontSize = 11;
                    if (GUILayout.Button("✘", GUILayout.Width(50), GUILayout.Height(18)))
                    {
                        parameterList.RemoveAt(i);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            EditorGUI.indentLevel--;

            var rect = GUILayoutUtility.GetRect(new GUIContent("button"), GUI.skin.button);
            rect.x += 45;
            rect.width = 30;
            rect.height = 30;
            GUI.skin.button.fontSize = 20;
            if (GUI.Button(rect, "+", GUI.skin.button))
            {
                apiInfo.AddNewParameter();
            }

            GUILayout.Space(10);
        }
    }
    public void CreateNewNameServerConfig()
    {
        var newServerConfig = ScriptableObject.CreateInstance<ServerConfig>();
        _serverConfig.CopyData(newServerConfig);

        AssetDatabase.DeleteAsset(NetworkResourcePath.ServerConfigDir + "/" + _serverConfig.name + ".asset");
        AssetDatabase.CreateAsset(newServerConfig, NetworkResourcePath.ServerConfigDir + "/" + newServerConfig.GetServerName() + ".asset");
        
        // AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        AssetDatabase.OpenAsset(newServerConfig.GetInstanceID());
    }

    public void CreateApiScripts()
    {
        APICreator.CreateAPIScripts(_serverConfig);
    }

    public void DrawSeparator(Color color)
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 1;

        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;
    }

    public string GetNameFromPath(string path)
    {
        string res = "";
        for (int i = 0; i < path.Length; i++)
        {
            if (path[i] == '/')
            {
                if (i == 0)
                {
                    i++;
                    if (i < path.Length)
                    {
                        char c = path[i];
                        if (c >= 97 && c <= 122)
                        {
                            c = (char)(c - 32);
                            res += c;
                        }
                        else
                        {
                            res += c;
                        }
                    }
                }
                else
                    res += "_";
            }
            else if (path[i] == '.')
            {
                res += "_";
            }
            else if (path[i] == '-')
            {
                i++;
                if (i < path.Length)
                {
                    char c = path[i];
                    if (c >= 97 && c <= 122)
                    {
                        c = (char)(c - 32);
                        res += c;
                    }
                }
            }
            else if (i == 0)
            {
                char c = path[i];
                if (c >= 97 && c <= 122)
                {
                    c = (char)(c - 32);
                    res += c;
                } else
                {
                    res += c;
                }
            }
            else
            {
                res += path[i];
            }
        }
        return res;
    }

    public void DeleteServerConfigFiles()
    {
        var path = NetworkResourcePath.ServerConfigDir + "/";
        
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles(_serverConfig.name + "*", SearchOption.AllDirectories);
        Debug.Log("config file name: " + _serverConfig.name);

        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log("delete file name: " + files[i].Name);
            AssetDatabase.DeleteAsset(path + files[i].Name);
        }
    }

    public void ShowDecodeMethod()
    {
        var decodeMethod_Prop = serializedObject.FindProperty("_decodeMethod");
        EditorGUILayout.PropertyField(decodeMethod_Prop);
        var decodeMethod = (ServerConfig.DecodeMethod)decodeMethod_Prop.enumValueIndex;
        if (decodeMethod == ServerConfig.DecodeMethod.Custom)
        {
            var decoder_Prop = serializedObject.FindProperty("_decoder");
            EditorGUILayout.ObjectField(decoder_Prop, new GUIContent("Custom Decoder"));
        }
    }

    public void SetAPIMaintainFileData()
    {

    }

    public void SetServerMaintainFileData()
    {
       
    }



    /*
    public bool ValidateDecoder(string decoderName)
    {
        //EditorMode下需要用到全名，以及Assembly.name ，可以通过GetType(Namespace.ClassName).AssemblyQualifiedName.ToString获取
        // Type classType = Type.GetType("Orcas.Networking.DefaultDecoder, Assembly-CSharp");
        //Type classType = Type.GetType("TestScript");
        Type classType = Type.GetType("Orcas.Networking.Decoder_errorTest, Assembly-CSharp");
        if (classType == null)
        {

            return false;
        }
        MethodInfo methodInfo = classType.GetMethod("Decode");

        if (methodInfo == null)
        {
            Debug.Log("classType: " + classType.ToString());
            Debug.Log("没有获取到Decode方法");
        }
        else
        {

            Debug.Log("before method Invoke");
            
            var obj = methodInfo.Invoke(new Decoder_errorTest(), new object[] { 123, "333" });  //调用静态函数的时候第一个参数为null
            var data = obj as IResponseData;
            if (data == null)
            {
                Debug.Log("Error! Method does not implement IResponseData Interface!");
            }
            else
            {
                Debug.Log("call func: " + decoderName + "." + "Decode");
              //  Debug.Log("result: data.code " + data.Code + "data.str: " + data.DataStr);
               
            }
        }

        return true;
    }
    */
}


