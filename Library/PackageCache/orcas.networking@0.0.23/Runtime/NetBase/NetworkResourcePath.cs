using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public static class NetworkResourcePath
    {
        public static string PathPrefix { get; private set; } = "Assets/Resources/Orcas/Networking";

        public static string ProtoDir { get; private set; } = PathPrefix + "/Protos";
        public static string CSProtoCreator_targetPath { get; private set; } = ProtoDir + "/CSharp";
        public static string LuaProtoCreator_targetPath { get; private set; } = ProtoDir + "/Lua";
        public static string ProtoMaintainDataPath { get; private set; } = ProtoDir + "/ProtosMaintainData.asset";



        public static string ProtoConfigDir { get; private set; } = PathPrefix + "/ProtosConfiguration";
        public static string CSProtoConfigDir { get; private set; } = ProtoConfigDir + "/CSharp";
        public static string LuaProtoConfigDir { get; private set; } = ProtoConfigDir + "/Lua";



        public static string ServerConfigDir { get; private set; } = PathPrefix + "/ServersConfiguration";
        public static string ServerCreator_targetPath { get; private set; } = PathPrefix + "/Servers/Http";
        public static string ServerMaintainDataPath { get; private set; } = PathPrefix + "/Servers/ServersMaintainData.asset";

        public static void SetPathPrefix(string str)
        {
            PathPrefix = str;
        }
    }
}
