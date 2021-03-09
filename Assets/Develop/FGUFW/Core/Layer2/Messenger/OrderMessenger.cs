using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace FGUFW.Core
{
    public class OrderMessenger<V> : IOrderMessenger<V>
    {
        Dictionary<string,Dictionary<Action<V>,int>> _eventDict = new Dictionary<string, Dictionary<Action<V>, int>>();
        HashSet<string> _aborts = new HashSet<string>();
        public void Abort(string msgID)
        {
            _aborts.Add(msgID);
        }

        public void Add(string msgID,int weight, Action<V> callback)
        {
            if(!_eventDict.ContainsKey(msgID))
            {
                _eventDict.Add(msgID,new Dictionary<Action<V>,int>());
            }
            var dict = _eventDict[msgID];
            if(dict.ContainsKey(callback))
            {
                Logger.w($"[OrderedMessenger.Add] msgID={msgID},消息重复监听");
            }
            else
            {
                dict.Add(callback,weight);
            }
            
        }

        public void Broadcast(string msgID, V msg)
        {
            if(_eventDict.ContainsKey(msgID))
            {
                _aborts.Remove(msgID);
                var dict = _eventDict[msgID];
                var dictSort = from kv in dict orderby kv.Value  descending select kv;
                foreach (var kv in dictSort)
                {
                    kv.Key(msg);
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
            if(_eventDict.ContainsKey(msgID) && _eventDict[msgID].ContainsKey(callback))
            {
                _eventDict[msgID].Remove(callback);
            }
        }
    }
}