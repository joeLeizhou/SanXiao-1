﻿using Orcas.Game.Multiplayer.Proto;

 namespace Orcas.Game.Multiplayer
{
    public interface ITeam
    {
        /// <summary>
        /// 队伍状态
        /// </summary>
        PlayerNetInfo[] Players { get; set; }
    }
}