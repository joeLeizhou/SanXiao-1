using UnityEngine;
using UnityEditor;
using Orcas.Networking;
using System.IO;

public class AddProtoEditor
{
    [MenuItem("Assets/Create/NetworkingConfig/AddCSProto")]
    static void CreateCSProtoConfig()
    {
        if (!Directory.Exists(NetworkResourcePath.ProtoConfigDir))
        {
            Directory.CreateDirectory(NetworkResourcePath.ProtoConfigDir);
        }

        ProtoConfig config = ScriptableObject.CreateInstance<ProtoConfig>();
        AssetDatabase.CreateAsset(config, NetworkResourcePath.CSProtoConfigDir + "/NewProto.asset");
        config.name = "NewProto";
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.OpenAsset(config.GetInstanceID());
    }

    [MenuItem("Assets/Create/NetworkingConfig/AddLuaProto")]
    static void CreateLuaProtoConfig()
    {
        if (!Directory.Exists(NetworkResourcePath.LuaProtoConfigDir))
        {
            Directory.CreateDirectory(NetworkResourcePath.LuaProtoConfigDir);
        }

        LuaProtoConfig config = ScriptableObject.CreateInstance<LuaProtoConfig>();
        AssetDatabase.CreateAsset(config, NetworkResourcePath.LuaProtoConfigDir + "/NewProto.asset");
        config.name = "NewProto";
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.OpenAsset(config.GetInstanceID());
    }
}

