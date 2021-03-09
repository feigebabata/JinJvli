using System;

namespace FGUFW.Core
{
    /// <summary>
    /// 有序广播 可以打断消息调用
    /// </summary>
    public interface IOrderMessenger<V>
    {
		void Add(string msgID,int weight,Action<V> callback);
		void Remove(string msgID,Action<V> callback);
		void Broadcast(string msgID,V msg);
        void Abort(string msgID);
    }
}