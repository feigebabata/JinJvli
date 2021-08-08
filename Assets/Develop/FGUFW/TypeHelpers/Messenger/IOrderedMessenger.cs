using System;

namespace FGUFW.Core
{
    /// <summary>
    /// 有序广播 可以打断消息调用
    /// </summary>
    public interface IOrderedMessenger<V>
    {
		void Add(string msgID,Action<V> callback,int weight=0);
		void Remove(string msgID,Action<V> callback);
		void Broadcast(string msgID,V msg);
        void Abort(string msgID);
    }
}