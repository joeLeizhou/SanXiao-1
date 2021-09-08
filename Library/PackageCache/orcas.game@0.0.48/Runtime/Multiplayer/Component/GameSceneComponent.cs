using Orcas.Ecs.Fsm;

namespace Orcas.Game.Multiplayer
{
    public struct GameSceneComponent : IFsmState
    {
        public int Stage;
        public int Scene;
        public SceneLoadState LoadState;
        public float StateEnterTime { get; set; }
        public uint DirtyFlag { get; set; }
        public uint DirtySystemIndex { get; set; }
    }
}