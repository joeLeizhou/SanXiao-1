using Orcas.Ecs.Fsm;

namespace Orcas.Game.Multiplayer
{
    public struct ServerObjectComponent : IFsmState
    {
        public long ServerId;
        public float StateEnterTime { get; set; }
        public uint DirtyFlag { get; set; }
        public uint DirtySystemIndex { get; set; }
    }
}