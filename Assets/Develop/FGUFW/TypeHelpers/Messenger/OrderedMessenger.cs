using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace FGUFW.Core
{
    public class OrderedMessenger<K,V> : IOrderedMessenger<K,V>
    {
        Dictionary<K,OrderedLinkedList<Action<V>>> _eventDict = new Dictionary<K, OrderedLinkedList<Action<V>>>();
        HashSet<K> _aborts = new HashSet<K>();
        public void Abort(K msgID)
        {
            _aborts.Add(msgID);
            LinkedList<object> linked = new LinkedList<object>();
        }

        public void Add(K msgID, Action<V> callback,int weight)
        {
            if(!_eventDict.ContainsKey(msgID))
            {
                _eventDict.Add(msgID,new OrderedLinkedList<Action<V>>());

            }
            var linked = _eventDict[msgID];
            if(linked.Contains(callback))
            {
                Debug.LogWarning($"[OrderedMessenger.Add] msgID={msgID},消息重复监听");
            }
            else
            {
                linked.Add(weight,callback);
            }
            
        }

        public void Broadcast(K msgID, V msg)
        {
            if(_eventDict.ContainsKey(msgID))
            {
                _aborts.Remove(msgID);
                var linked = _eventDict[msgID];
                foreach (var kv in linked)
                {
                    kv.Value(msg);
                    if(_aborts.Contains(msgID))
                    {
                        _aborts.Remove(msgID);
                        break;
                    }
                }
            }
        }

        public void Remove(K msgID, Action<V> callback)
        {
            if(_eventDict.ContainsKey(msgID) && _eventDict[msgID].Contains(callback))
            {
                _eventDict[msgID].Remove(callback);
            }
        }
    }
}