using Orcas.Resources;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HotFixTest : MonoBehaviour
{
    private int _currentPack = 0;
    private void OnHotfixState(HotfixState state)
    {
        Debug.Log("hotfix " + state);
        if (state == HotfixState.InstallOver)
        {
            HotFixManager.Instance.CheckHotfix(OnHotfixState);
        }
        else if (state == HotfixState.DownOver)
        {
            _currentPack = 1;
            PackHotfixManager.Instance.CheckPack(_currentPack, OnSceneHotState);
        }
    }

    private void OnSceneHotState(PackHotfixState state)
    {
        Debug.Log("scene hot " + state + " pack " + _currentPack);
        if (state == PackHotfixState.NeedDownload)
        {
            PackHotfixManager.Instance.StartDownload(_currentPack, OnSceneHotState);
        }
        else if (state == PackHotfixState.DownloadOver)
        {
            if (_currentPack < 3)
            {
                _currentPack++;
                PackHotfixManager.Instance.CheckPack(_currentPack, OnSceneHotState);
            }
            else
            {
                // Debug.Log("add test");
                // gameObject.AddComponent<ScriptsFromFile>();
            }
        }
        else
        {
                // Debug.Log("add test");
                // gameObject.AddComponent<ScriptsFromFile>();
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        string testServer = $"file:///{Path.GetFullPath("TestServer/")}";
        Debug.Log("test server " + testServer);
        PathConst.ServerUrlArr = new string[] { testServer };
        HotFixManager.Instance.CheckInstall(OnHotfixState);
        PackHotfixManager.Instance.PullAllFileList(new List<int> { 1, 2, 3 }, null);
    }

    // Update is called once per frame
    void Update()
    {
        HotFixManager.Instance.OnUpdate();
    }
}
