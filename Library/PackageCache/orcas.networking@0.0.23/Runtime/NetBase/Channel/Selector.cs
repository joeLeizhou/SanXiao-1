// using System.Collections.Generic;
//
// namespace Orcas.Networking
// {
//     public abstract class Selector
//     {
//         protected List<Channels> Channels;
//
//         protected abstract void InitChannels();
//         protected abstract Channels SelectS2C(ByteBuffer buffer);
//         protected abstract Channels SelectC2S(int protoId);
//
//         protected Selector()
//         {
//             Channels = new List<Channels>();
//             InitChannels();
//         }
//
//         public List<Channels> GetChannelsList()
//         {
//             return Channels;
//         }
//         
//         internal void ServerToClient(ByteBuffer buffer)
//         {
//             SelectS2C(buffer).ServerToClient(buffer);
//         }
//
//         internal byte[] ClientToServer(int protoId, object protoData)
//         {
//             return SelectC2S(protoId).ClientToServer(protoData);
//         }
//     }
//
//     public class DefaultSelector : Selector
//     {
//         protected override void InitChannels()
//         {
//             
//         }
//
//         protected override Channels SelectS2C(ByteBuffer buffer)
//         {
//             return null;
//         }
//
//         protected override Channels SelectC2S(int protoId)
//         {
//             return null;
//         }
//     }
// }