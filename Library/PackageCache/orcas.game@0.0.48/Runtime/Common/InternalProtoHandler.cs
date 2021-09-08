//using System.Collections.Generic;
//using Orcas.Networking;
//using UnityEngine;

//namespace Orcas.Game.Common
//{
//    public class InternalProtoHandler : ChannelHandler<IRltProto[], IRltProto[]>
//    {
//        private readonly List<IRltProto> _listOfProto = new List<IRltProto>();
//        protected override void Server2Client(ChannelContext context, out IRltProto[] data1, in IRltProto[] data2)
//        {
//            _listOfProto.Clear();
//            for (var i = 0; i < data2.Length; i++)
//            {
//                switch (data2[i].ID)
//                {
//                    case CommonProtoId.RltLogin:
//                    {
//                        var proto = data2[i];
//                        GameManager.Instance.DoActionInMainThread(() =>
//                        {
//                            GameManager.Instance.GetManager<LoginManager>().InvokeLoginCallBack(proto as RltLogin);
//                        });
//                        break;
//                    }
//                    default:
//                    {
//                        _listOfProto.Add(data2[i]);
//                        break;
//                    }
//                }
//            }
//            data1 = _listOfProto.ToArray();
//        }
//    }
//}