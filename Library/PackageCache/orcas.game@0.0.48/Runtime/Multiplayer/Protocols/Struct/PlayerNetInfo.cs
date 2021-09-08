using System;

namespace Orcas.Game.Multiplayer.Proto
{
    public class PlayerNetInfo
    {
        public int Id { get; set; }
        //public int TeamIndex { get; set; }
        public PlayerRoomState RoomState { get; set; }
        public PlayerNetState NetState { get; set; }
    }
}