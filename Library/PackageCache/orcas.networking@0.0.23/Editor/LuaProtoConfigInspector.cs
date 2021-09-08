using UnityEngine;
using UnityEditor;
using Orcas.Networking;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

[CustomEditor(typeof(LuaProtoConfig)), CanEditMultipleObjects]
public class LuaProtoConfigInspector : Editor
{

    public struct PathNode
    {
        public int Parent; //父节点在链表中的位置
        public int Self;   //在ProtoDataType[]中的位置

        public PathNode(int self, int parent)
        {
            this.Self = self;
            this.Parent = parent;
        }
    }

    private LuaProtoConfig _protoConfig;
    private bool[] _visible = null;
    int currentVisibleItem = 0;
    private List<PathNode> _pathNodes;


    private void OnEnable()
    {
        _visible = new bool[1000];
        _pathNodes = new List<PathNode>();
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _pathNodes = new List<PathNode>();
        currentVisibleItem = 0;
        _protoConfig = (LuaProtoConfig)target;
        var protoName_Prop = serializedObject.FindProperty("_protoName");
        EditorGUILayout.PropertyField(protoName_Prop);
        //base.OnInspectorGUI();
        EditorGUILayout.Space(5);

        DrawSeparator(Color.gray);

        var protoDataArray_Prop = serializedObject.FindProperty("_protoDataArray");
        ProtoDataTypeListInterator(protoDataArray_Prop, ref GetVisibleItem(), -1, true);

        

        serializedObject.ApplyModifiedProperties();

        DrawSeparator(Color.green);
        EditorGUILayout.Space(30);
        GUI.skin.button.fontSize = 20;
        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("保存", GUILayout.Width(80), GUILayout.Height(50))) {
                if (!_protoConfig.CheckData()) return;

                ProtosMaintainer.DeleteLuaProto(_protoConfig.name);
                ProtosMaintainer.AddProto(_protoConfig);
                if (_protoConfig.name != _protoConfig.GetProtoName())
                    CreateNewNameProtoConfig();
            }

            EditorGUILayout.Space(50);

            if (GUILayout.Button("删除", GUILayout.Height(50), GUILayout.Width(100)))
            {
                ProtosMaintainer.DeleteProto(_protoConfig.name);

                DeleteProtoConfigFiles();
            }
        }
        EditorGUILayout.EndHorizontal();
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
        var newProtoConfig = ScriptableObject.CreateInstance<LuaProtoConfig>();
        _protoConfig.CopyData(newProtoConfig);
        AssetDatabase.DeleteAsset(NetworkResourcePath.LuaProtoConfigDir + "/" + _protoConfig.name + ".asset");
        AssetDatabase.CreateAsset(newProtoConfig, NetworkResourcePath.LuaProtoConfigDir + "/" + newProtoConfig.GetProtoName() + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        AssetDatabase.OpenAsset(newProtoConfig.GetInstanceID());
    }
    private void DeleteProtoConfigFiles()
    {
        var path = NetworkResourcePath.LuaProtoConfigDir;
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

    void ProtoDataTypeListInterator(SerializedProperty listProperty, ref bool visible, int parentIndex, bool showAddButton)
    {
        visible = EditorGUILayout.Foldout(visible, "Data List");

        if (visible)
        {
            EditorGUI.indentLevel++;

            for (int i = listProperty.arraySize - 1; i >= 0 ; i--)
            {
                _pathNodes.Add(new PathNode(i, parentIndex));
                int l = _pathNodes.Count;
                SerializedProperty protoDataProperty = listProperty.GetArrayElementAtIndex(i);
                ProtoDataTypeField(protoDataProperty, ref GetVisibleItem(), l - 1);
                GUILayout.Space(10);
            }


            if (showAddButton)
            {
                var rect = GUILayoutUtility.GetRect(new GUIContent("button"), GUI.skin.button);
                rect.x += 45;
                rect.width = 30;
                rect.height = 30;
                GUI.skin.button.fontSize = 20;
                if (GUI.Button(rect, "+", GUI.skin.button))
                {
                    AddArrayItem(parentIndex);
                    // apiInfo.AddNewParameter();
                }
            }

            EditorGUI.indentLevel--;

            GUILayout.Space(10);
        }
    }

    void ProtoDataTypeField(SerializedProperty dataProperty, ref bool visible, int nodeIndex)
    {
        bool deleted = false;
        EditorGUILayout.BeginHorizontal();
        {
            visible = EditorGUILayout.Foldout(visible, "Data");

            GUI.skin.button.fontSize = 11;

            if (GUILayout.Button("✘", GUILayout.Width(50), GUILayout.Height(18)))
            {
                RemoveItem(nodeIndex);
                DeleteVisibleItem(currentVisibleItem - 1);
                deleted = true;
                return;
            }

        }
        EditorGUILayout.EndHorizontal();
        if (deleted) return;
        // LuaTypeCode typeCode;

        if (visible)
        {
            EditorGUI.indentLevel++;

            dataProperty.Next(true);
            // EditorGUILayout.LabelField(new GUIContent("name"), GUILayout.Width(100));
            if (ShouldShowName(nodeIndex))
                EditorGUILayout.PropertyField(dataProperty);

            GUILayout.Space(15);

            dataProperty.Next(false);
            //EditorGUILayout.LabelField(new GUIContent("type"), GUILayout.Width(100));

            //typeCode = (LuaTypeCode)dataProperty.intValue;
            if (ShouldShowType(nodeIndex))
            {
                EditorGUILayout.PropertyField(dataProperty);
            }
            else
            {
                SetEmptyType(nodeIndex);
            }


            //  GUILayout.Space(5);
            GUILayout.Space(10);

            dataProperty.Next(false);
            if (ShouldShowEnum(nodeIndex))
            {
                EditorGUILayout.PropertyField(dataProperty);
            }
            else if (ShouldShowTable(nodeIndex))
            {
                var count = CheckTableCount(nodeIndex);
                dataProperty.Next(false);
                ProtoDataTypeListInterator(dataProperty, ref GetVisibleItem(), nodeIndex, (count == -1) ? true : false);


            }

            EditorGUI.indentLevel--;

            GUILayout.Space(10);
            DrawSeparator(Color.yellow);
        }
    }

    ref bool GetVisibleItem()
    {
        int index = currentVisibleItem++;

        return ref _visible[index];
    }

    void DeleteVisibleItem(int index)
    {
        var list = _visible.ToList();
        list.RemoveAt(index);
        _visible = list.ToArray();
    }

    bool ShouldShowName(int nodeIndex)
    {
        var data = GetParentData(nodeIndex);
        if (data == null) return true;

        if (data.Type == LuaTypeCode.BasicArray) return false;

        if (data.Type == LuaTypeCode.VariantArray && _pathNodes[nodeIndex].Self != 0)
            return false;

        return true;
    }

    bool ShouldShowType(int nodeIndex)
    {
        var data = GetParentData(nodeIndex);
        if (data == null) return true;

        if (data.Type == LuaTypeCode.VariantArray && _pathNodes[nodeIndex].Self != 0)
            return false;

        return true;
    }

    void SetEmptyType(int nodeIndex)
    {
        var data = GetSelfData(nodeIndex);
        if (data != null)
            data.Type = LuaTypeCode.Empty;
    }

    bool ShouldShowTable(int nodeIndex)
    {
        var code = GetSelfData(nodeIndex).Type;
        //Debug.Log("typeCode: " + code.ToString());
        Debug.Log("Code.Type: " + code.ToString());
        switch (code)
        {
            case LuaTypeCode.Object:
            case LuaTypeCode.BasicArray:
            case LuaTypeCode.CustomArray:
            case LuaTypeCode.VariantArray:
                return true;
            default:
                break;
        }
        
        if (_pathNodes[nodeIndex].Self != 0 && GetParentData(nodeIndex)?.Type == LuaTypeCode.VariantArray)
            return true;
        return false;
    }

    int CheckTableCount(int nodeIndex)
    {
        int count = -1;
        var selfData = GetSelfData(nodeIndex);

        if (selfData.Type == LuaTypeCode.Object || selfData.Type == LuaTypeCode.CustomArray)
        {
            count = -1;
        }
        else if (selfData.Type == LuaTypeCode.BasicArray)
        {
            count = 1;
        }
        else if (selfData.Type == LuaTypeCode.VariantArray)
        {

            count = (selfData.Table == null || selfData.Table.Length == 0 || selfData.Table[0].Enums == null) ? 1 : selfData.Table[0].Enums.Length + 1;
        }

        if (count != -1)
        {
            Debug.Log("Check table count: " + count);
            selfData.CheckTableCount(count);
        }

        return count;
    }

    bool ShouldShowEnum(int nodeIndex)
    {
        var data = GetParentData(nodeIndex);
        if (data == null) return false;
       
        return (data.Type == LuaTypeCode.VariantArray) && (_pathNodes[nodeIndex].Self == 0) ? true : false;
    }

    void AddArrayItem(int nodeIndex)
    {
        if (nodeIndex == -1)
        {
            _protoConfig.AddNewItem();
        }
        else
        {
            var data = GetSelfData(nodeIndex);
            data.AddNewItem();
        }
    }

    void RemoveItem(int nodeIndex)
    {
        var selfIndex = _pathNodes[nodeIndex].Self;
        Debug.Log("Delete SelfIndex: " + selfIndex);
        if (_pathNodes[nodeIndex].Parent == -1)
        {
            _protoConfig.RemoveItem(selfIndex);
        }
        else
        {
            var data = GetParentData(nodeIndex);
            data.RemoveItem(selfIndex);
        }
    }

    LuaProtoConfig_DataType GetParentData(int nodeIndex)
    {
        var currentNode = _pathNodes[nodeIndex];
        nodeIndex = currentNode.Parent;

        var paths = new Stack<int>();
        while (nodeIndex != -1)
        {
            var pathNode = _pathNodes[nodeIndex];
            paths.Push(pathNode.Self);

            nodeIndex = pathNode.Parent;
        }

        var dataArray = _protoConfig.GetProtoDataArray();

        LuaProtoConfig_DataType data = null;

        while (!(paths.Count == 0))
        {
            int index = paths.Pop();
            data = dataArray[index];
            dataArray = data.Table;
        }

        return data;
    }

    LuaProtoConfig_DataType GetSelfData(int nodeIndex)
    {
        var selfIndex = _pathNodes[nodeIndex].Self;
        var data = GetParentData(nodeIndex);
        LuaProtoConfig_DataType self;
        if (data == null)
        {
            self = _protoConfig.GetProtoDataArray()[selfIndex];
        }
        else
        {
            self = data.Table[selfIndex];
        }
        return self;
    }



}
