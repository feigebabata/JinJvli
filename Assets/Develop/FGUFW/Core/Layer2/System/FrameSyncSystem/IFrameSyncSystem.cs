using System;
using Google.Protobuf;

namespace FGUFW.Core
{
    public interface IFrameSyncSystem :ISystem
    {
        void PushCmd(uint cmd,IMessage message);

        Action<LogicFrame> OnLogicFrameUpdate { get;set;}
    }
}