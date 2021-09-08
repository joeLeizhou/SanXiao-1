using System.IO;
using UnityEngine;

namespace  Orcas.Graph.Core
{
    public interface IGraphFileLoader
    {
        byte[] LoadGraphFile(string file);
    }

    public class DefaultGraphFileLoader : IGraphFileLoader
    {
        public byte[] LoadGraphFile(string path)
        {
            if (path.StartsWith("Assets") == false && path.StartsWith("Resources") == false)
            {
                path = "Assets/Resources/" + path;
            }

            var fileName = Path.GetFileNameWithoutExtension(path);
            path = Path.GetDirectoryName(Path.GetFullPath(path));
            path = path + '/' + fileName + "b.bytes";
            //Debug.Log("Load Graph:" + path);
            return File.ReadAllBytes(path);
        }
    }
}