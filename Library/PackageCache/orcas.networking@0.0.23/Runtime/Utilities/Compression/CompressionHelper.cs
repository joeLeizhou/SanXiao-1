using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using UnityEngine;
namespace Orcas.Networking
{
   public class CompressionHelper
   {
      private static int compressCount = 0;

      public static int MaxStreamLength = 65536;
      public static byte[] ZipCompress(byte[] rawData)
      {
         using (MemoryStream ms = new MemoryStream())
         {
            using (GZipOutputStream zipStream = new GZipOutputStream(ms))
            {

               zipStream.Write(rawData, 0, rawData.Length);
               zipStream.IsStreamOwner = false;
               zipStream.Flush();
               zipStream.Close();
               var result = ms.ToArray();
               return result;
            }            
         }
      }

      public static byte[] ZipDecompress(byte[] zipData)
      {
         using (GZipInputStream zipStream = new GZipInputStream(new MemoryStream(zipData)))
         {
   //         zipStream.GetNextEntry();
            MemoryStream re = new MemoryStream(50000);
            int count;
            byte[] data = new byte[50000];
            while ((count = zipStream.Read(data, 0, data.Length)) != 0)
            {
               re.Write(data, 0, count);
            }
            byte[] overarr = re.ToArray();
            return overarr; 
         }
      }
      
      public static byte[] ZipDecompress(byte[] zipData, int bufferIndex, int bufferLen)
      {
         using (GZipInputStream zipStream = new GZipInputStream(new MemoryStream(zipData, bufferIndex, bufferLen)))
         {
            MemoryStream re = new MemoryStream(50000);
            int count;
            byte[] data = new byte[50000];
            while ((count = zipStream.Read(data, 0, data.Length)) != 0)
            {
               re.Write(data, 0, count);
            }
            byte[] overarr = re.ToArray();
            return overarr; 
         }
      }
   }
}
