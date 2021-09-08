using Orcas.Networking.Tcp;
using Newtonsoft.Json.Linq;

namespace Orcas.Networking
{
    public class ReqLuaMessage : IReqProto
    {
        public ushort ID { get; set; }
        public string Data { get; set; }
    }
}
