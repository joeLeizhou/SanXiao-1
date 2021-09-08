using UnityEngine;
using UnityEditor;
using Orcas.Networking;
using System.IO;

public class AddServerEditor
{
  [MenuItem("Assets/Create/NetworkingConfig/ServerConfig")]
  static void CreateServerConfig()
  {
        ServerConfig config = ScriptableObject.CreateInstance<ServerConfig>();
        if (!Directory.Exists(NetworkResourcePath.ServerConfigDir))
        {
            Directory.CreateDirectory(NetworkResourcePath.ServerConfigDir);
        }
        AssetDatabase.CreateAsset(config, NetworkResourcePath.ServerConfigDir + "/UnNamedServer.asset");
        config.name = "UnNamedServer";
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssetDatabase.OpenAsset(config.GetInstanceID());
    }
}

