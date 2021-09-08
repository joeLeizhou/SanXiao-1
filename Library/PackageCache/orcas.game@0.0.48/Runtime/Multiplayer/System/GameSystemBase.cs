using Orcas.Ecs.Fsm;
using Orcas.Networking;

namespace Orcas.Game.Multiplayer
{
    public abstract class GameSystemBase : FsmSystemBase
    {
        protected static IClient BattleServer;
        protected static IRoom Room;

        public static void SetServer(IClient battleServer)
        {
            BattleServer = battleServer;
        }

        public static void SetRoom(IRoom room)
        {
            Room = room;
        }
    }
}