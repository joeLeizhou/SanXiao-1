using Orcas.Networking.Tcp;
using Orcas.Core.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Orcas.Networking
{
    public class ByteBuffer : Core.Tools.ByteBuffer
    {
        public int Length()
        {
            return length;
        }
        
        public byte[] GetBytes()
        {
            return bytes;
        }
        
        public IRltProto[] ReadProtocol()
        {
            if (length < ProtocolFactory.PROTO_HEADER_SIZE + ProtocolFactory.PROTO_ID_SIZE) return null; // 正常包头长度为  包长(2bit)+协议号(2bit)
            var bytesCount = length;
            List<IRltProto> protocols = new List<IRltProto>();
            int curReadCount = 0;
            while (curReadCount < bytesCount) // 如果出现黏包，会有多个协议在缓冲区，需要多次读取
            {
                var startIndex = rIndex;
                var protocolLen = BitConverter.ToInt32(bytes, rIndex);
                if (protocolLen < ProtocolFactory.PROTO_ID_SIZE)
                {
                    UnityEngine.Debug.LogError("协议解析出错，协议长度小于2");
                    break;
                }

                curReadCount += ProtocolFactory.PROTO_HEADER_SIZE;
                if (bytesCount - curReadCount < protocolLen) // 当前缓冲区内数据不完整，可能出现分包
                {
                    UnityEngine.Debug.LogError("协议解析出错，缓冲区长度小于协议长度 " + protocolLen.ToString());
                    break;
                }
                rIndex += ProtocolFactory.PROTO_HEADER_SIZE;

                var id = BitConverter.ToUInt16(bytes, rIndex);
                rIndex += ProtocolFactory.PROTO_ID_SIZE;

                IRltProto protocol;
                if (ProtocolFactory.Instance.IsCompressionProto(id))
                {
                    var tempBytes = CompressionHelper.ZipDecompress(base.bytes, rIndex, protocolLen - ProtocolFactory.PROTO_ID_SIZE);
                    rIndex += protocolLen - ProtocolFactory.PROTO_ID_SIZE;
                    var byteBuffer = new ByteBuffer(tempBytes.Length);
                    byteBuffer.WriteBytes(tempBytes, tempBytes.Length);
                    protocol = ProtocolFactory.Instance.CreateProto(id, byteBuffer, tempBytes.Length + 2);
                }
                else
                {
                    protocol = ProtocolFactory.Instance.CreateProto(id, this, protocolLen);
                }
                if (protocol != null)
                {
                    protocols.Add(protocol);
                }
                else
                {
                    UnityEngine.Debug.LogError("null proto " + id);
                }

                curReadCount += protocolLen;

                var protocolLenAddHeader = protocolLen + ProtocolFactory.PROTO_HEADER_SIZE;

                if (rIndex - startIndex != protocolLenAddHeader)
                {
                  //  DebugHelper.LogError("ID " + id.ToString() + " 读取不一致 已读:" + (rIndex - startIndex).ToString() + " protocolLen:" + (protocolLenAddHeader).ToString());
                    rIndex = startIndex + protocolLenAddHeader;
                }
                length -= protocolLenAddHeader;
            }

            rIndex = rIndex % size;
            return protocols.ToArray();
        }

        public void Debug128(string tag)
        {
          //  DebugHelper.Log("buffer " + rIndex.ToString() + " " + tag + " " + BitConverter.ToString(bytes, rIndex, 128));
        }

        public ByteBuffer(int size) : base(size)
        {
        }
    }
}
