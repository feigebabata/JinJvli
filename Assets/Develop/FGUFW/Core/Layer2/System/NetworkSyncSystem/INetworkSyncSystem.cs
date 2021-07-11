namespace FGUFW.Core
{
    public interface INetworkSyncSystem : ISystem
    {
        void SendMsg(object msg,uint cmd);
    }
}