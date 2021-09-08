using System;
using UnityEngine;

namespace Orcas.Networking
{
    public class JsonArrayHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
            return wrapper.array;
        }
 
        [Serializable]
        public class Wrapper<T>
        {
            public T[] array;
        }
    }    
}

