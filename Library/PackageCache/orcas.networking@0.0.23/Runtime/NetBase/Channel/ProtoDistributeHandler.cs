using UnityEngine;
namespace Orcas.Networking
{
    public class ProtoDistributeHandler : ChannelHandler<object, IRltProto[]>
    {
        protected override void Server2Client(ChannelContext context, out object data1, in IRltProto[] data2)
        {
            data1 = null;
            var protos = data2;
            NetworkUpdate.Instance.AddActionDoInMainThread(() =>
            {
                for (int i = 0; i < protos.Length; i++)
                {
                    if (ProtocolFactory.Instance.CheckIsLuaProto(protos[i].ID))
                        Channel.Client.ClientEventHandler.OnReceiveLuaMessage(protos[i] as RltLuaMessage);
                    else
                        Channel.Client.ClientEventHandler.OnReceiveMessage(protos[i]);
                }
            });
        }
    }
}