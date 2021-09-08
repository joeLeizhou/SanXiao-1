using Orcas.Ecs.Fsm;

namespace Orcas.Game.Multiplayer
{
    public struct RoomStateComponent : IFsmState
    {
        public RoomState State;
        public float StateEnterTime { get; set; }
        public uint DirtyFlag { get; set; }
        public uint DirtySystemIndex { get; set; }
    }
}