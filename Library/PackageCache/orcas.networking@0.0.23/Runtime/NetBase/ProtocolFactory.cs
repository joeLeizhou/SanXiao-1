using System;
using System.CodeDom;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using Orcas.Networking.Tcp;
using Orcas.Core.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Orcas.Networking
{
    public class ProtocolFactory
    {
        // public delegate Tools.ByteBuffer ProtoBufferInputHandler(ushort id, Tools.ByteBuffer buffer, int protocolLen);
        // public delegate byte[] ProtoBytesOutputHandler(byte[] bytes);
        private static ProtocolFactory _instance;
        public static ProtocolFactory Instance
        {
            get
            {
                if (_instance == null) _instance = new ProtocolFactory();
                return _instance;
            }
        }
        private Dictionary<ushort, ConstructorInfo> constructorsDict;
        private Dictionary<ushort, PropertyInfo[]> propsDict;
        private Dictionary<ushort, ProtoDataType> propsJsonDict;
        private HashSet<ushort> compressionProtoSet;
        private object[] constructorParams;
        // public ProtoBufferInputHandler inputHandler;
        // public ProtoBytesOutputHandler outputHandler;

        private ProtocolFactory()
        {
            constructorParams = new object[0];
            constructorsDict = new Dictionary<ushort, ConstructorInfo>();
            propsDict = new Dictionary<ushort, PropertyInfo[]>();
            propsJsonDict = new Dictionary<ushort, ProtoDataType>();
            compressionProtoSet = new HashSet<ushort>();
        }

        internal bool IsCompressionProto(ushort id)
        {
            return compressionProtoSet.Contains(id);
        }

        /// <summary>
        /// 根据 PropertyIndexAttribute 排序
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private PropertyInfo[] SortProperty(PropertyInfo[] properties)
        {
            var keys = new int[properties.Length];
            var hasIndex = false;
            for (int i = 0; i < properties.Length; i++)
            {
                keys[i] = 0;
                var indexAttribute = properties[i].GetCustomAttribute<PropertyIndexAttribute>();
                if (indexAttribute != null)
                {
                    keys[i] = indexAttribute.Index;
                    hasIndex = true;
                }
            }
            if (hasIndex)
                Array.Sort(keys, properties);
            return properties;
        }

        /// <summary>
        /// 添加协议定义
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        public void AddProto<T>(ushort id, bool overwrite = true, bool zipCompression = false) where T : IProtocol
        {
            AddProto(id, typeof(T), overwrite, zipCompression);
        }

        /// <summary>
        /// 添加协议类定义
        /// 相同id重复添加会覆盖之前添加的内容
        /// </summary>
        /// <param name="id">协议号</param>
        /// <param name="protoType">协议类</param>
        public void AddProto(ushort id, Type protoType, bool overwrite = true, bool zipCompression = false)
        {
            if (zipCompression && compressionProtoSet.Contains(id) == false)
            {
                compressionProtoSet.Add(id);
            }
            if (constructorsDict.ContainsKey(id))
            {
                if (overwrite)
                {
                    constructorsDict[id] = protoType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null);
                    propsDict[id] = SortProperty(protoType.GetProperties());
                }
            }
            else
            {
                constructorsDict.Add(id, protoType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null));
                propsDict.Add(id, SortProperty(protoType.GetProperties()));
            }
        }

        /// <summary>
        /// 添加lua层协议定义， 格式固定为 ProtoDataType
        /// </summary>
        /// <param name="id"></param>
        /// <param name="typeJson"></param>
        public void AddLuaProto(ushort id, string typeJson, bool zipCompression = false)
        {
            var protocol = JsonConvert.DeserializeObject<ProtoDataType>(typeJson);
            if (zipCompression && compressionProtoSet.Contains(id) == false)
            {
                compressionProtoSet.Add(id);
            }
            if (propsJsonDict.ContainsKey(id))
                propsJsonDict[id] = protocol;
            else
                propsJsonDict.Add(id, protocol);
        }

        internal IRltProto CreateCSProto(ushort id, Core.Tools.ByteBuffer buffer, int protocolLen)
        {
            if (constructorsDict.ContainsKey(id) == false)
            {
                UnityEngine.Debug.LogError("proto " + id + " not add");
                return null;
            }
            var ret = constructorsDict[id].Invoke(constructorParams) as IRltProto;
            ret.ID = id;
            // UnityEngine.Debug.Log("proto " + ret.ID + ":" + ret.GetType());
            // var props = propsDict[id];
            SerializeTools.FillObject(ret, propsDict[id], buffer, 1);
            return ret;
        }

        internal IRltProto CreateLuaProto(ushort id, Core.Tools.ByteBuffer buffer, int protocolLen)
        {
            var ret = new RltLuaMessage();
            ret.ID = id;
            //UnityEngine.Debug.Log("proto " + id);
            // var props = propsJsonDict[id];
            SerializeJsonTools.FillObject(ret.Data, propsJsonDict[id], buffer, 1);
            return ret;
        }

        /// <summary>
        /// 从bytebuffer解析协议
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="protocolLen"></param>
        /// <returns></returns>
        public IRltProto CreateProto(ushort id, Core.Tools.ByteBuffer buffer, int protocolLen)
        {
            // if (inputHandler != null)
            //     buffer = inputHandler(id, buffer, protocolLen);

            if (CheckIsLuaProto(id))
                return CreateLuaProto(id, buffer, protocolLen);
            else
                return CreateCSProto(id, buffer, protocolLen);
        }

        public byte[] GetCSProtocolBytes(IProtocol protocol)
        {
            var bytes = new List<byte>();
            var props = propsDict[protocol.ID];
            for (int i = 1; i < props.Length; i++)
            {
                var prop = props[i].GetValue(protocol);
                bytes.AddRange(SerializeTools.GetObjectBytes(prop));
            }
            return bytes.ToArray();
        }

        public byte[] GetLuaProtocolBytes(ReqLuaMessage protocol)
        {
            var bytes = new List<byte>();
            var props = propsJsonDict[protocol.ID];
            var data = JObject.Parse(protocol.Data);
            for (int i = 1; i < props.Table.Length; i++)
            {
                bytes.AddRange(SerializeJsonTools.GetObjectBytes(data, props.Table[i]));
            }
            return bytes.ToArray();
        }

        // internal byte[] GetProtocolBytes(IProtocol protocol)
        // {
        //     byte[] bytes;
        //     if (CheckIsLuaProto(protocol.ID))
        //         bytes = GetLuaProtocolBytes((ReqLuaMessage)protocol);
        //     else
        //         bytes = GetCSProtocolBytes(protocol);
        //
        //     if (outputHandler != null)
        //         bytes = outputHandler(bytes);
        //     return bytes;
        // }
        /// <summary>
        /// 协议
        /// </summary>
        public static int PROTO_HEADER_SIZE = 4;
        public static int PROTO_ID_SIZE = 2;

        /// <summary>
        /// 协议序列化为byte数组
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        // public byte[] GetBytes(IProtocol protocol)
        // {
        //     var bID = BitConverter.GetBytes(protocol.ID);
        //     byte[] bytes = GetProtocolBytes(protocol);
        //     var bLen = BitConverter.GetBytes(bytes.Length + PROTO_ID_SIZE);
        //     var ret = new byte[bytes.Length + PROTO_HEADER_SIZE + PROTO_ID_SIZE];
        //     Buffer.BlockCopy(bLen, 0, ret, 0, PROTO_HEADER_SIZE);
        //     Buffer.BlockCopy(bID, 0, ret, PROTO_HEADER_SIZE, PROTO_ID_SIZE);
        //     Buffer.BlockCopy(bytes, 0, ret, PROTO_HEADER_SIZE + PROTO_ID_SIZE, bytes.Length);
        //     return ret;
        // }

        public bool CheckIsLuaProto(ushort ID)
        {
            return propsJsonDict.ContainsKey(ID);
        }

        ///
        /// 清理lua层协议定义
        /// 
        public void ClearLuaProto()
        {
            if (propsJsonDict != null)
            {
                propsJsonDict.Clear();
            }
        }
    }
}
