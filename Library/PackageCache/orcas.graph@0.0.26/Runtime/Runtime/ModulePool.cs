using System.Collections.Generic;
using System;

namespace Orcas.Graph.Core
{

    public class ActivatorPool<T>
    {
        private Queue<T> queue;
        private Type tType;
        public ActivatorPool(Type t)
        {
            this.tType = t;
            this.queue = new Queue<T>();
        }

        public T GetObject()
        {
            if (queue.Count > 0)
                return queue.Dequeue();
            else
                return (T)Activator.CreateInstance(tType);
        }

        public void CollectObject(T t)
        {
            queue.Enqueue(t);
        }
        public void Clear()
        {
            queue.Clear();
        }
        private static Dictionary<Type, ActivatorPool<T>> pools = new Dictionary<Type, ActivatorPool<T>>();
        public static T GetObject(Type t)
        {
            if (pools.ContainsKey(t) == false)
            {
                pools.Add(t, new ActivatorPool<T>(t));
            }
            return pools[t].GetObject();
        }

        public static void CollectObject(Type moduleType, T module)
        {
            if (pools.ContainsKey(moduleType))
                pools[moduleType].CollectObject(module);
        }

        public static void ClearAll()
        {
            foreach (var item in pools)
            {
                item.Value.Clear();
            }
            pools.Clear();
        }
    }
}