using System;

namespace FGUFW.Core
{
    /// <summary>
    /// 有序广播 可以打断消息调用
    /// </summary>
    public interface IOrderedMessenger<K,V>
    {
		void Add(K msgID,Action<V> callback,int weight=0);
		void Remove(K msgID,Action<V> callback);
		void Broadcast(K msgID,V msg);
        void Abort(K msgID);
    }
}