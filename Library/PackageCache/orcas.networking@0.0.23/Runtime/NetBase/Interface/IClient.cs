using System.Threading.Tasks;

namespace Orcas.Networking
{
    public interface IClient
    {
        IClientEventHandler ClientEventHandler { get; set; }
        NetworkState NetworkState { get;}
        ClientOption Option { get; set; }
        Channel Channel { get; set; }
        void SendMessage(IReqProto message);
        Task<bool> Connect();
        void DisConnect();
        void SetClientOption(ClientOption option);
    }
}