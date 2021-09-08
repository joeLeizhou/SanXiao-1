using Orcas.Ecs.Fsm;

namespace Orcas.Game.Multiplayer
{
    public struct PlayerComponent : IFsmState
    {
        public PlayerNetState NetState;
        public PlayerRoomState RoomState;
        public float StateEnterTime { get; set; }
        public uint DirtyFlag { get; set; }
        public uint DirtySystemIndex { get; set; }
    }
}