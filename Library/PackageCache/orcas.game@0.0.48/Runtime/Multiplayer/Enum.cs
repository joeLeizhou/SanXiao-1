namespace Orcas.Game.Multiplayer
{
    public enum RoomState : byte
    {
        //server state start
        WaitForPlayerJoinRoom = 0,
        WaitForPlayerStartGame = 1,
        Running = 2,
        Pause = 3,
        GameOver = 4,
        //server state end
        //local state start
        WaitForSelfEnterGame = 63,
        //local state end
    }

    public enum SceneLoadState : byte
    {
        Idle = 0,
        Loading = 1,
        Loaded = 2
    }

    public enum PlayerNetState : byte
    {
        Connected = 0,
        Disconnected = 1
    }

    public enum PlayerRoomState : byte
    {
        Joining = 0,
        JoinedRoom = 1,
        ReqStartGame = 2,
        QuitRoom = 3
    }

    public enum GameStateReason : byte
    {
        None = 0,
        ClientCantJoinRoom = 1,
        ClientDisconnect = 2,
    }
}