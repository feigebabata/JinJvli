using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace FGUFW.Core.System
{
    abstract public class OrderMessenger<T, Y> : IOrderMessenger<T, Y>
    {
        Dictionary<T,Dictionary<Action<Y>,int>> _eventDict = new Dictionary<T, Dictionary<Action<Y>,int>>();
        HashSet<T> _aborts = new HashSet<T>();
        public void Abort(T msgID)
        {
            _aborts.Add(msgID);
        }

        public void Add(T msgID,int weight, Action<Y> callback)
        {
            if(!_eventDict.ContainsKey(msgID))
            {
                _eventDict.Add(msgID,new Dictionary<Action<Y>,int>());
            }
            var dict = _eventDict[msgID];
            if(dict.ContainsKey(callback))
            {
                Debug.LogWarningFormat("[OrderedMessenger.Add] msgID={0},消息重复监听",msgID);
            }
            else
            {
                dict.Add(callback,weight);
            }
            
        }

        public void Broadcast(T msgID, Y msg)
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

        public void Remove(T msgID, Action<Y> callback)
        {
            if(_eventDict.ContainsKey(msgID) && _eventDict[msgID].ContainsKey(callback))
            {
                _eventDict[msgID].Remove(callback);
            }
        }
    }
}