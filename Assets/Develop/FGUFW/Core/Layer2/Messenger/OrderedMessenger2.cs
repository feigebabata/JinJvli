using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FGUFW.Core
{
    public class OrderedMessenger2<V> : IOrderedMessenger<V>
    {
        Dictionary<string,OrderedLinkedList<Action<V>>> _eventDict = new Dictionary<string, OrderedLinkedList<Action<V>>>();
        HashSet<string> _aborts = new HashSet<string>();
        public void Abort(string msgID)
        {
            _aborts.Add(msgID);
            LinkedList<object> linked = new LinkedList<object>();
        }

        public void Add(string msgID, Action<V> callback,int weight)
        {
            if(!_eventDict.ContainsKey(msgID))
            {
                _eventDict.Add(msgID,new OrderedLinkedList<Action<V>>());

            }
            var linked = _eventDict[msgID];
            if(linked.Contains(callback))
            {
                Logger.w($"[OrderedMessenger.Add] msgID={msgID},消息重复监听");
            }
            else
            {
                linked.Add(weight,callback);
            }
            
        }

        public void Broadcast(string msgID, V msg)
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

        public void Remove(string msgID, Action<V> callback)
        {
            if(_eventDict.ContainsKey(msgID) && _eventDict[msgID].Contains(callback))
            {
                _eventDict[msgID].Remove(callback);
            }
        }
    }
}