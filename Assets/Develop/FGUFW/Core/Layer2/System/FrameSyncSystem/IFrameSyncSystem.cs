using System;
using System.Collections.Generic;
using Google.Protobuf;

namespace FGUFW.Core
{
    public interface IFrameSyncSystem :ISystem
    {
        void PushCmd(uint cmd,IMessage message);
        List<LogicFrame> LogicFrames{get;}
        Action<LogicFrame> OnLogicFrameUpdate { get;set;}
    }
}