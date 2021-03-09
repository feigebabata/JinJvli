using System;
using System.Collections.Generic;

namespace FGUFW.Core
{
    /// <summary>
    /// 事件分发中枢
    /// </summary>
    /// <typeparam name="V"></typeparam>
    public class Messenger<K,V> : IMessenger<K,V>
    {
        Dictionary<K,Action<V>> _eventDict = new Dictionary<K, Action<V>>();
        public void Add(K msgID, Action<V> callback)
        {
            if(_eventDict.ContainsKey(msgID))
            {
                if(Array.IndexOf(_eventDict[msgID].GetInvocationList(),callback) != -1)
                {
                    Logger.w($"[Messenger.Add] msgID={msgID},消息重复监听");
                }
                else
                {
                    _eventDict[msgID] += callback;
                }
            }
            else
            {
                _eventDict.Add(msgID,callback);
            }
        }

        public void Broadcast(K msgID, V msg)
        {
            if(_eventDict.ContainsKey(msgID))
            {
                _eventDict[msgID](msg);
            }
        }

        public void ClearAll()
        {
            _eventDict.Clear();
        }

        public void Remove(K msgID, Action<V> callback)
        {
            if(_eventDict.ContainsKey(msgID))
            {
                _eventDict[msgID] -= callback;
            }
        }
    }
}
