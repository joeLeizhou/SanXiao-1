namespace Orcas.Game.Multiplayer.Proto
{
    public static class MultiPlayerProtoId
    {
        public const ushort ReqEnterRoom = 4007;
        public const ushort ReqStartGame = 4011;
        public const ushort RltRoundStart = 4014;
        public const ushort ReqReconnect = 4015;
        public const ushort RltReconnect = 4016;
        public const ushort ReqRenterGame = 4017;
        public const ushort RltRenterGame = 4018;

        public const ushort ReqQuitGame = 4021;
        public const ushort RltPlayerState = 4022;
        public const ushort RltRoomState = 4024;

        public const ushort ReqHeartBeat = 5101;
        public const ushort RltHeartBeat = 5102;
    }
}