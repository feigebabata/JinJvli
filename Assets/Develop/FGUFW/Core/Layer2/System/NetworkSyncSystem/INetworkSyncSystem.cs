using System;
using Google.Protobuf;

namespace FGUFW.Core
{
    public interface INetworkSyncSystem : ISystem
    {
        void SendMsg(uint cmd,ushort gameplayID,object msg);
        IMessenger<uint,ByteString> Messenger{get;}
    }
}