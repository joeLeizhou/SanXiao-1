using System;
using System.Collections.Generic;
using UnityEngine;

namespace Orcas.Networking
{
    public class NetworkUpdate : MonoBehaviour
    {
        private Queue<Action> _actions;
        private static NetworkUpdate _instance;
        private static bool _destroyed = false;
        public static NetworkUpdate Instance
        {
            get
            {
                if (_instance == null && _destroyed == false)
                {
                    var gameObject = new GameObject("Network");
                    _instance = gameObject.AddComponent<NetworkUpdate>();
                    _instance._actions = new Queue<Action>();
                    DontDestroyOnLoad(gameObject);
                }

                return _instance;
            }
        }

        public delegate void NetwrokUpdate();

        public NetwrokUpdate UpdateEvent;

        public delegate void NetworkDestory();

        public NetworkDestory DestoryEvent;

        private void OnDestroy()
        {
            if (DestoryEvent != null)
            {
                DestoryEvent();
            }
            _instance = null;
            _destroyed = true;
        }

        internal void Init()
        {
        }

        internal void AddActionDoInMainThread(Action action)
        {
            lock (_actions)
            {
                _actions.Enqueue(action);
            }
        }

        void Update()
        {
            if (_actions != null)
            {
                while (_actions.Count > 0)
                {
                    Action action = null;
                    lock (_actions)
                    {
                        action = _actions.Dequeue();
                    }
                    action?.Invoke();
                }
            }

            if (UpdateEvent != null)
            {
                UpdateEvent();
            }
        }
    }
}