// using System;
// using System.Net.Sockets;
// using System.Net;
// using System.IO;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using UnityEngine;
// using Orcas.Core.Tools;
// using Orcas.Networking;
//
// namespace Orcas.Networking.Tcp
// {
//
//     public class TcpMockClient
//     {
//
//         public static string rootPath = Application.persistentDataPath + "/Record/";
//         private string readFilePath;
//         private string writeFilePath;
//         private FileStream readStream = null;
//         private FileStream writeStream = null;
//         private int lastReadTime = 0;
//         private ByteBuffer byteBuffer;
//         private byte[] buffer = new byte[2048];
//         private Thread readThread = null;
//         private int writeIndex;
//
//         public TcpMockClient()
//         {
//             byteBuffer = new ByteBuffer(2048);
//             if (!Directory.Exists(rootPath))
//             {
//                 Directory.CreateDirectory(rootPath);
//             }
//         }
//
//
//         public void StartReadCommand(string fileName)
//         {
//             this.readFilePath = rootPath + fileName;
//             CloseRead();
//             readStream = File.OpenRead(readFilePath);
//            // this.readThread = new Thread(new ThreadStart(ReadMessage));
//             this.readThread.Start();
//             lastReadTime = 0;
//         }
//
//         public void CloseRead()
//         {
//            // DebugHelper.Log("CloseRead");
//             // if (tcpClient == null)
//             //     return;
//             if (readStream != null)
//                 readStream.Close();
//             if (readThread != null)
//                 readThread.Abort();
//             // tcpClient.Close();
//             // tcpClient = null;
//             readStream = null;
//             readThread = null;
//         }
//
//         /*
//         public void ReadMessage()
//         {
//             while (true)
//             {
//                 try
//                 {
//                     if (BattleCommandHelper.BattleStartFrame > 0 && UnityEngine.XR.WSA.WorldManager.FrameCount > lastReadTime - 5)
//                     {
//                         int allBytes = 0;
//                         while (allBytes < 256)
//                         {
//                             int bytes = readStream.Read(buffer, allBytes, 4);
//                             if (bytes < 4)
//                                 break;
//                             var protocolLen = BitConverter.ToUInt16(buffer, allBytes) - 2;
//                             allBytes += 4;
//                             bytes = readStream.Read(buffer, allBytes, protocolLen);
//                             if (bytes < protocolLen)
//                                 break;
//                             allBytes += protocolLen;
//                         }
//                         if (allBytes > 0)
//                         {
//                             byteBuffer.WriteBytes(buffer, allBytes);
//                             var protos = byteBuffer.ReadProtocol(allBytes);
//                             if (protos != null && protos.Length > 0)
//                             {
//                                 var proto = protos[protos.Length - 1] as IBattleProtocol;
//                                 lastReadTime = proto.Time + BattleCommandHelper.BattleStartFrame;
//                                 GameManager.Instance.AddActionDoInMainThread(() =>
//                                 {
//                                     for (int i = 0; i < protos.Length; i++)
//                                     {
//                                         protos[i].Deal();
//                                     }
//                                 });
//                             }
//                         }
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     UnityEngine.Debug.LogError("tcp数据接收错误：" + e.Message + " " + e.StackTrace);
//                     CloseRead();
//                     break;
//                 }
//
//                 Thread.Sleep(20);
//             }
//         }
//
//         */
//
//         public void CloseWrite()
//         {
//             if (writeStream == null)
//                 return;
//           //  DebugHelper.Log("CloseWrite");
//             writeStream.Close();
//             writeStream = null;
//         }
//
//         public void StartWriteCommand(string fileName)
//         {
//             this.writeFilePath = rootPath + fileName;
//            // DebugHelper.Log("StartWrite: " + writeFilePath);
//             CloseWrite();
//             this.writeStream = File.Open(this.writeFilePath, FileMode.Truncate, FileAccess.Write);
//             this.writeIndex = 0;
//
//         }
//         public void WriteCommand(IProtocol protocol)
//         {
//             var bytes = ProtocolFactory.Instance.GetBytes(protocol);
//             if (writeStream != null)
//             {
//                 writeStream.Write(bytes, 0, bytes.Length);
//                 writeIndex += bytes.Length;
//                 writeStream.Flush();
//             }
//             else
//             {
//                 UnityEngine.Debug.LogError("stream is null!");
//             }
//         }
//     }
// }
