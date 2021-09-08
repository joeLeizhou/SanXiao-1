using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Orcas.Networking;

namespace Orcas.Game.Multiplayer.Proto
{
    public abstract class ReqRenterGame : IReqProto
    {
        public ushort ID { get; set; } = MultiPlayerProtoId.ReqRenterGame;
    }

    public class DefaultReqRenterGame : ReqRenterGame
    {

    }
}