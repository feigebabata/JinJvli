using System;

namespace FGUFW.Core.System
{
    /// <summary>
    /// 有序广播 可以打断消息调用
    /// </summary>
    public interface IOrderMessenger<T,Y>
    {
		void Add(T msgID,int weight,Action<Y> callback);
		void Remove(T msgID,Action<Y> callback);
		void Broadcast(T msgID,Y msg);
        void Abort(T msgID);
    }
}