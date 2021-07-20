using System;
using Google.Protobuf;

namespace FGUFW.Core
{
    public interface INetworkSyncSystem : ISystem
    {
        void SendMsg(uint cmd,long gameplayID,object msg);
        IMessenger<uint,ByteString> Messenger{get;}
    }
}