using UnityEngine;
using UnityEditor;
using Orcas.Networking;
using System.IO;

[CustomEditor(typeof(ProtoConfig)), CanEditMultipleObjects]
public class ProtoConfigInspector : Editor
{
    private ProtoConfig _protoConfig;

    enum Choice
    {
        item1,
        item2,
        item3
    }

    float _scrollViewWidth = 300;
    float _selectionGridWidth = 500;

    int _choiceIndex = 0;
    Vector2 _drawerPos;

    string _oldFocusName = "";
    bool _hasSelectedValue = false;
    int _controlCount = 0;
    void Ping()
    {
        var focusName = GUI.GetNameOfFocusedControl();
        if (focusName != _oldFocusName)
        {
            _oldFocusName = GUI.GetNameOfFocusedControl();
            Debug.Log("focusName: " + _oldFocusName);

        }
        if (focusName == "")
        {
            if (_protoConfig._helperIndex != -1)
            {
                Debug.Log("set Index to -1!--------");
                _protoConfig.SetNewHelperIndex(-1);
                _hasSelectedValue = false;
            }
        }
        else if (focusName == "SelectionGrid" || focusName == "ScrollView")
        {
            Debug.Log("focus is on SelectionGrid");
        }
        else
        {
            var newIndex = int.Parse(focusName);

            if (newIndex != _protoConfig._helperIndex)
            {
                //  _drawerPos = new Vector2(0, 0);
                _drawerPos = new Vector2(0.5f * (_selectionGridWidth - _scrollViewWidth), 0);    //（selectiongrid的宽度 - scrollview的宽度）* 0.5，让其显示在中间位置
                _protoConfig.SetNewHelperIndex(newIndex);
                _hasSelectedValue = false;
                _choiceIndex = -1;
            }
            else if (_choiceIndex != -1 && !_hasSelectedValue)
            {
                HasSelectedType();
            }

        }
    }

    void CheckEditing()
    {
        if (EditorGUIUtility.editingTextField)
        {
            //  Debug.Log("is Editing!");
            if (_hasSelectedValue == true)
            {
                _hasSelectedValue = false;
                Debug.Log("_hasSelectedValue = " + _hasSelectedValue);
            }
        }
    }
    void HasSelectedType()
    {
        // _protoConfig.SetParaType(_protoConfig.DisplayTypes.ToArray()[_choiceIndex]);

        _protoConfig.SetSelectedValue(_protoConfig.DisplayTypes.ToArray()[_choiceIndex]);
        _choiceIndex = -1;
        _hasSelectedValue = true;
        _protoConfig.SetNewHelperIndex(-1);
        GUI.FocusControl("");
    }

    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();  //放在最上面
                                    //  Ping();
        _protoConfig = (ProtoConfig)target;
        CheckEditing();
         Ping();
        _controlCount = 0;

        var protoName_Prop = serializedObject.FindProperty("_protoName");
        EditorGUILayout.PropertyField(protoName_Prop);

        var protoType_Prop = serializedObject.FindProperty("_protoType");
        EditorGUILayout.PropertyField(protoType_Prop);

        var language_Prop = serializedObject.FindProperty("_language");
        EditorGUILayout.PropertyField(language_Prop);

        ParaListIterator("_paraList", ref _protoConfig.ParaListVisible);

      //  CheckEditing();
       // Ping();
        //serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(40);
        GUI.skin.button.fontSize = 20;
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("保存", GUILayout.Height(40), GUILayout.Width(100)))
            {
                if (!_protoConfig.CheckData()) return;

                 ProtosMaintainer.DeleteProto(_protoConfig.name);
                

                ProtosMaintainer.AddProto(_protoConfig);
                if (_protoConfig.name != _protoConfig.GetProtoName())
                CreateNewNameProtoConfig();
                
            }

            GUILayout.Space(50);

            if (GUILayout.Button("删除", GUILayout.Height(40), GUILayout.Width(100)))
            {
                ProtosMaintainer.DeleteProto(_protoConfig.name);
          
                DeleteProtoConfigFiles();  
            }
        }
        GUILayout.EndHorizontal();
    }

    void ParaListIterator(string propertyPath, ref bool visible)
    {
        SerializedProperty listProperty = serializedObject.FindProperty(propertyPath);
        visible = EditorGUILayout.Foldout(visible, "Parameter List");
        EditorGUI.indentLevel++;
        if (visible)
        {
            DrawSeparator(Color.yellow);
            //EditorGUI.indentLevel++;

            for (int i = 0; i < listProperty.arraySize; i++)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
                    elementProperty.Next(true);

                    EditorGUILayout.LabelField(new GUIContent("type"), GUILayout.Width(65));
                    // if (i == _protoConfig._helperIndex && !_hasSelectedValue)
                    // {
                    if (_controlCount == _protoConfig._helperIndex && !_hasSelectedValue)
                    {
                        EditorGUILayout.BeginVertical();
                        {

                            GUI.SetNextControlName(_controlCount++.ToString());
                            //var rect = EditorGUILayout.GetControlRect();
                            //rect.width = 250;
                            // EditorGUI.PropertyField(rect, elementProperty, new GUIContent(""));
                            EditorGUILayout.PropertyField(elementProperty, new GUIContent(""), GUILayout.Width(250));
                            //_protoConfig.EditingString = elementProperty.stringValue;


                            //  _drawerPos = EditorGUILayout.BeginScrollView(_drawerPos, true, true, GUILayout.Width(250), GUILayout.Height(120));
                            _drawerPos = EditorGUILayout.BeginScrollView(_drawerPos, true, true, GUILayout.Width(_scrollViewWidth), GUILayout.Height(120));
                            {

                                GUI.SetNextControlName("SelectionGrid");

                                //  _choiceIndex = GUILayout.SelectionGrid(_choiceIndex, _protoConfig.DisplayTypes.ToArray(), 1, EditorStyles.miniButtonLeft, GUILayout.Width(230));
                                _choiceIndex = GUILayout.SelectionGrid(_choiceIndex, _protoConfig.DisplayTypes.ToArray(), 1, EditorStyles.miniButtonLeft, GUILayout.Width(_selectionGridWidth));
                            }
                            EditorGUILayout.EndScrollView();

                        }
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        //不能都放在BeginVertical里面，否则会报错
                            GUI.SetNextControlName(_controlCount++.ToString());
                            EditorGUILayout.PropertyField(elementProperty, new GUIContent(""), GUILayout.Width(250));
                    }

                    elementProperty.Next(false);
                    EditorGUILayout.LabelField(new GUIContent("name"), GUILayout.Width(65));
                    EditorGUILayout.PropertyField(elementProperty, new GUIContent(""), GUILayout.Width(150));

                    GUILayout.Space(30);
                    GUI.skin.button.fontSize = 16;
                    GUI.skin.button.fontStyle = FontStyle.Bold;

                    EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(10));

                    if (GUILayout.Button("✘", GUILayout.Width(24), GUILayout.Height(24)))
                    {
                        _protoConfig.DeleteParaItem(i);
                        continue;
                    }

                    EditorGUILayout.LabelField(new GUIContent(""), GUILayout.Width(10));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                DrawSeparator(Color.grey);
            }

            GUILayout.Space(20);

            GUI.skin.button.fontStyle = FontStyle.Bold;
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("✚", GUILayout.Height(30), GUILayout.Width(80)))
                {
                    _protoConfig.AddParaItem();
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

    void DrawSeparator(Color color)
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

    private void CreateNewNameProtoConfig()
    {
        var newProtoConfig = ScriptableObject.CreateInstance<ProtoConfig>();
        _protoConfig.CopyData(newProtoConfig);
        AssetDatabase.DeleteAsset(NetworkResourcePath.ProtoConfigDir + "/" + _protoConfig.name + ".asset");
        AssetDatabase.CreateAsset(newProtoConfig, NetworkResourcePath.ProtoConfigDir + "/" + newProtoConfig.GetProtoName() + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        AssetDatabase.OpenAsset(newProtoConfig.GetInstanceID());
    }

    private void DeleteProtoConfigFiles()
    {
        var path = NetworkResourcePath.ProtoConfigDir;
        DirectoryInfo directory = new DirectoryInfo(path);
        FileInfo[] files = directory.GetFiles(_protoConfig.name + "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            Debug.Log("delete file name: " + files[i].Name);
            AssetDatabase.DeleteAsset(path + "/" + files[i].Name);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
