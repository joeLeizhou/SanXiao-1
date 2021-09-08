using Orcas.Ecs.Fsm.Interface;

namespace Orcas.Game.Multiplayer
{
    public class EnterGameSceneSystem : GameSystemBase
    {
        public ISceneHelper SceneHelper;
        private IFsm _myFsm;

        protected override void OnUpdate()
        {
            if (_myFsm == null)
            {
                _myFsm = Room.GetPlayerFsm(Room.MultiPlayer.MyServerId);
            }

            //GameSceneComponent sceneComp;
            //var tryEnterGame = _myFsm.GetState<PlayerComponent>().RoomState == PlayerRoomState.JoinedRoom;
            //if (tryEnterGame)
            //{
            //    var state = Room.RoomFsm.GetState<RoomStateComponent>();
            //    tryEnterGame = (state.State == RoomState.WaitForPlayerStartGame || state.State == RoomState.Running ||
            //                    state.State == RoomState.GameOver);

            //    if (tryEnterGame)
            //    {
            //        sceneComp = Room.RoomFsm.GetState<GameSceneComponent>();
            //        if (Room.RoomFsm.GetState<GameSceneComponent>().LoadState == SceneLoadState.Idle)
            //        {
            //            SceneHelper.LoadScene(sceneComp.Stage, sceneComp.Scene);
            //            sceneComp.LoadState = SceneLoadState.Loading;
            //            SetState(Room.RoomFsm, sceneComp);
            //        }
            //    }
            //}

            //var loaded = false;
            //sceneComp = Room.RoomFsm.GetState<GameSceneComponent>();
            //if (SceneHelper.LoadState == SceneLoadState.Loaded &&
            //    sceneComp.LoadState == SceneLoadState.Loading)
            //{
            //    sceneComp.LoadState = SceneLoadState.Idle;
            //    loaded = true;
            //}

            //if (loaded)
            //{
            //    SetState(_myFsm, new PlayerComponent() {RoomState = PlayerRoomState.ReqStartGame}); /////// JoinedRoom
            //    Room.ReqStartGame();
            //}
        }
    }
}