using Orcas.Ecs.Fsm;

namespace Orcas.Game.Multiplayer
{
    public struct GameRoundComponent : IFsmState
    {
        public int CurrentRound;
        public int CurrentRoundPlayer;
        public float StateEnterTime { get; set; }
        public uint DirtyFlag { get; set; }
        public uint DirtySystemIndex { get; set; }
    }
}