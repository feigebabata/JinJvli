using Google.Protobuf;

namespace FGUFW.Core
{
    public interface INetworkSyncSystem : ISystem
    {
        void SendMsg(uint cmd,ushort gameplayID,object msg);
        IMessenger<uint,ByteString> Messenger{get;}
    }

    static public class NetworkConfig
    {
        public const ushort APP_ID = 333;
        public const ushort PACK_HEAD_LENGTH = 6;
        public const ushort PACK_GAMEPLAY_LENGTH = 2;
        public const ushort PACK_APPID_LENGTH = 2;
        public const ushort PACK_LEN_LENGTH = 2;
        public const int BROADCAST_COUNT = 1;
    }
}