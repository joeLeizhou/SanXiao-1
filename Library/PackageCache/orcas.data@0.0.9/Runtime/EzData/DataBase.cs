using System;
using System.IO;
using Orcas.Core;
using Orcas.Core.Tools;
using UnityEngine;

namespace Orcas.Data
{
    public abstract class DataBase
    {
        protected float LastSaveTime;
        public void Save(string name)
        {
            var bytes = SerializeTools.GetObjectBytes(this);
            var path = Application.persistentDataPath + Path.DirectorySeparatorChar + name;
            try
            {
                var fs = File.OpenWrite(path);
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
                fs.Close();
                LastSaveTime = Time.time;
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static T Load<T>(string name) where T : DataBase
        {
            var path = Application.persistentDataPath + Path.DirectorySeparatorChar + name;
            if (File.Exists(path) == false) return null;
            var fs = File.OpenRead(path);
            var bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            var byteBuffer=  new ByteBuffer(bytes.Length);
            byteBuffer.WriteBytes(bytes, bytes.Length);
            if (!(SerializeTools.GetObject(typeof(T), byteBuffer) is T ret)) return null;
            ret.LastSaveTime = Time.time;
            return ret;
        }
    }
}