using UnityEngine;
using LuaInterface;
using Orcas.Core;

namespace SanXiao.Game
{
    public class LuaStateManager : IManager
    {
        //主要管理customluaClient,luaState,luaLooper操作
        private bool hasInit = false;

        public bool HasInit
        {
            get { return hasInit; }
            set { hasInit = value; }
        }

        private LuaState _luaState;
        private CustomLuaClient _luaClient;
        private LuaLooper _luaLooper;
        private GameObject _gameObject;

        public void Init()
        {
            if (HasInit == false)
            {
                _gameObject = new GameObject("LuaClient");
                var client = _gameObject.AddComponent<CustomLuaClient>();
                GameObject.DontDestroyOnLoad(_gameObject);
                _luaState = CustomLuaClient.GetMainState();
                _luaClient = LuaClient.Instance as CustomLuaClient;
                _luaLooper = client.GetLooper();
                //_luaClient.LuaFileMgrInit(_luaState);

                // InitUIManagerData();
                InitGlobal();
                HasInit = true;

                GameLauncher.Instance.luaLateUpdate = _luaState.GetFunction("LuaLateUpdate");
            }
        }

        private void InitUIManagerData()
        {
            //--初始化UIManager
            LuaFunction f_Init = _luaState.GetFunction("UIManager.Init");
            f_Init.Call();
            //-设置更新间隔
            LuaFunction f_SetDeltaTime = _luaState.GetFunction("UIManager.SetDeltaTime");
            f_SetDeltaTime.Call(GameLooper.DELTA_TIME);
        }

        private void InitGlobal()
        {
#if TEST
            this._luaState.DoString("TEST = true");
#endif
#if ADD_LOG
            this._luaState.DoString("ADD_LOG = true");
#endif
        }

        public LuaTable GetGameLuaTable(string fullPath)
        {
            return _luaState.GetTable(fullPath);
        }

        public LuaFunction GetGameLuaFunction(string fullPath)
        {
            return _luaState.GetFunction(fullPath);
        }

        public object GetGameLuaTableValue(LuaTable table, string field)
        {
            if (table == null || string.IsNullOrEmpty(field))
                return null;
            return table[field];
        }

        public void DestroyAll()
        {
            LuaFunction df = _luaState.GetFunction("UIManager.DestroyAllUI");
            df?.Call();
        }

        public void OnDestroy()
        {
            GameObject.Destroy(_gameObject);
        }

        public void OnPause()
        {
            _luaState?.GetFunction("OnApplicationPause")?.Call<bool>(true);
        }

        public void OnResume()
        {
            _luaState?.GetFunction("OnApplicationPause")?.Call<bool>(false);
        }

        public void Update(uint currentFrameCount)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// 一个临时的update
        /// </summary>
        public void TempUpdate()
        {
            _luaLooper.LuaUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}
