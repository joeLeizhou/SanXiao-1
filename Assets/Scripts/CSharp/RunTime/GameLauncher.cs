using System.Collections;
using System.Collections.Generic;
using LuaInterface;
using SanXiao.Core;
using Orcas.Core;
using Orcas.Fsm;
using Orcas.Graph.Core;
using Orcas.Resources;
using Orcas.Core;
using UnityEngine;
using Orcas.Csv;
using Orcas.Data;
using Orcas.Game;
using Orcas.Fsm;
using Orcas.Game.Common;
using Orcas.Graph.Core;
using Orcas.Networking;
using Orcas.Resources;
using SanXiao.Game;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    // Start is called before the first frame update
    public LuaFunction luaLateUpdate;
    public static GameLauncher Instance {get;set;}
    private bool needHotFixUpdate = false;
    public bool _hasInit = false;
    public StateMachine _stateMachine;
    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        _hasInit = false;
        DontDestroyOnLoad(gameObject);
        Instance = this;
        GameManager.Instance.Init();
        Application.targetFrameRate = 60;
        // #if UNITY_ANDROID && !UNITY_EDITOR
        // Screen.orientation = ScreenOrientation.LandscapeRight;
        // #elif UNITY_IOS && !UNITY_EDITOR
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SoundLoader.AudioFilePath = "Assets/External/Audio/";
        GameManager.Instance.AddManager<GraphManager>();
    }

    private void Start() {
        //一些预处理，基本可以先不管
#if ADD_LOG
        SRDebug.Init();
#endif
#if !UNITY_EDITOR
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
#endif

        GameManager.Instance.SetLooper(new GameLooper());
        _stateMachine = new StateMachine();
        StateFactory.Instance.AddState<GameStateInit>();
        GameManager.Instance.AddManager<LuaStateManager>();
        StateFactory.Instance.AddState<GameStateLua>();
        Debug.Log("ssss============ss");
        _stateMachine.ChangeState<GameStateInit>();

 
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_hasInit)
        {
            _stateMachine.Update();
        }
    }
    
    public void ClearAndRestart()
    {
       
    }

    public async void ClearResources()
    {
        
    }
    
    public static void RestartApp()
    {
        
    }

    public static void ClearCacheAndRestartApp()
    {
      
    }

}
