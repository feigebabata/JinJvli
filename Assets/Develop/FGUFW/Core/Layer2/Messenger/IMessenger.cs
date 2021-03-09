using System;

namespace FGUFW.Core
{
	/// <summary>
	/// 事件分发中枢
	/// </summary>
	/// <typeparam name="V"></typeparam>
	public interface IMessenger<K,V>
	{
		void Add(K msgID,Action<V> callback);

		void Remove(K msgID,Action<V> callback);
		
		void Broadcast(K msgID,V msg);
		void ClearAll();
	}
}
