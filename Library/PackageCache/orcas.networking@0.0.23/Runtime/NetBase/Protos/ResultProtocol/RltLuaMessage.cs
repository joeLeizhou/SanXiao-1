using Orcas.Networking.Tcp;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Orcas.Networking
{

    public class RltLuaMessage : IRltProto
    {
        public RltLuaMessage()
        {
            Data = new JObject();
        }
        public ushort ID { get; set; }

        public JObject Data { get; set; }

        public void Deal()
        {
      //      TcpClientHelper.OnMessage(this);
        }
    }
}