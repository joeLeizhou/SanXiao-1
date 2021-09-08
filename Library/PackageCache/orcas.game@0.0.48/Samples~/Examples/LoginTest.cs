using Orcas.Game;
using Orcas.Game.Common;
using Orcas.Game.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginTest : MonoBehaviour
{
    public ConnectType ConnectType = ConnectType.Tcp;
    public string IP = "10.0.104.186";
    public int Port = 9999;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Init();
        GameManager.Instance.SetLooper(new GameLogicLooper());
        var gmMgr = GameManager.Instance.AddManager<GameClientManager>();
        var lgMgr = GameManager.Instance.AddManager<LoginManager>();
        lgMgr.SetLoginListener((proto) =>
        {
            Debug.Log("login success");
        });
        gmMgr.Connect(ConnectType, IP, Port, (connected) =>
        {
            if (connected == false) return;
            lgMgr.Login(LoginType.FaceBook, "");
        }, lgMgr);

        gmMgr.SetRltProtoEvent(1001, (iProto) =>
        {
            var rltLogin = iProto as RltLogin;
            var str = rltLogin.ID.ToString() + "\n" + rltLogin.Id.ToString() + "\n" + rltLogin.ResCode.ToString() + "\n" + rltLogin.Binder.Value.ToString() + "\n" + rltLogin.ServerTime.ToString() + "\n" + rltLogin.Token.ToString() + "\n";
            Debug.Log("fzy rlt Res:" + str);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
