using Unity.Entities;

namespace Orcas.Game.Multiplayer
{
    public struct TeamSharedComponent : ISharedComponentData
    {
        public int TeamIndex;

        public static TeamSharedComponent Team0 = new TeamSharedComponent() { TeamIndex = 0 };
        public static TeamSharedComponent Team1 = new TeamSharedComponent() { TeamIndex = 1 };
        public static TeamSharedComponent Team2 = new TeamSharedComponent() { TeamIndex = 2 };
        public static TeamSharedComponent Team3 = new TeamSharedComponent() { TeamIndex = 3 };
        public static TeamSharedComponent Team4 = new TeamSharedComponent() { TeamIndex = 4 };

        public static TeamSharedComponent[] Teams = new TeamSharedComponent[] { Team0, Team1, Team2, Team3, Team4 };
    }
}