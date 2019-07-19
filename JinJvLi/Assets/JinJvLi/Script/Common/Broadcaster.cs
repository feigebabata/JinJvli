using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件广播工具 用于功能集之间解耦
/// </summary>
public static class Broadcaster
{
    static Dictionary<string, Delegate> m_events = new Dictionary<string, Delegate>(); 

    public static void Clear(string _key=null)
    {
        if(_key==null)
        {
            m_events.Clear();
        }
        else
        {
            if(m_events.ContainsKey(_key))
            {
                m_events.Remove(_key);
            }
        }
    }

    public static void Add(string _key,Action _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action callBack = m_events[_key] as Action;
                callBack+=_callBack;
                m_events[_key] = callBack;
            }
            else
            {
                addErr(_key);
            }
        }
        else
        {
            addEvent(_key,_callBack);
        }
    }

    public static void Add<T>(string _key,Action<T> _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action<T> callBack = m_events[_key] as Action<T>;
                callBack+=_callBack;
                m_events[_key] = callBack;
            }
            else
            {
                addErr(_key);
            }
        }
        else
        {
            addEvent(_key,_callBack);
        }
    }

    public static void Add<T,U>(string _key,Action<T,U> _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action<T,U> callBack = m_events[_key] as Action<T,U>;
                callBack+=_callBack;
                m_events[_key] = callBack;
            }
            else
            {
                addErr(_key);
            }
        }
        else
        {
            addEvent(_key,_callBack);
        }
    }

    public static void Add<T,U,I>(string _key,Action<T,U,I> _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action<T,U,I> callBack = m_events[_key] as Action<T,U,I>;
                callBack+=_callBack;
                m_events[_key] = callBack;
            }
            else
            {
                addErr(_key);
            }
        }
        else
        {
            addEvent(_key,_callBack);
        }
    }

    public static void Remove(string _key,Action _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action callBack = m_events[_key] as Action;
                callBack-=_callBack;
                if(callBack== null)
                {
                    removeEvent(_key);
                }
                else
                {
                    m_events[_key] = callBack;
                }
            }
            else
            {
                removeErr(_key);
            }
        }
    }

    public static void Remove<T>(string _key,Action<T> _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action<T> callBack = m_events[_key] as Action<T>;
                callBack-=_callBack;
                if(callBack== null)
                {
                    removeEvent(_key);
                }
                else
                {
                    m_events[_key] = callBack;
                }
            }
            else
            {
                removeErr(_key);
            }
        }
    }

    public static void Remove<T,U>(string _key,Action<T,U> _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action<T,U> callBack = m_events[_key] as Action<T,U>;
                callBack-=_callBack;
                if(callBack== null)
                {
                    removeEvent(_key);
                }
                else
                {
                    m_events[_key] = callBack;
                }
            }
            else
            {
                removeErr(_key);
            }
        }
    }

    public static void Remove<T,U,I>(string _key,Action<T,U,I> _callBack)
    {
        if(m_events.ContainsKey(_key))
        {
            if(equalsType(m_events[_key],_callBack))
            {
                Action<T,U,I> callBack = m_events[_key] as Action<T,U,I>;
                callBack-=_callBack;
                if(callBack== null)
                {
                    removeEvent(_key);
                }
                else
                {
                    m_events[_key] = callBack;
                }
            }
            else
            {
                removeErr(_key);
            }
        }
    }

    public static void Broadcast(string _key)
    {
        if(m_events.ContainsKey(_key))
        {
            Action e = m_events[_key] as Action;
            if(e!= null)
            {
                e();
            }
            else
            {
                broadcastErr(_key);
            }
        }
    }

    public static void Broadcast<T>(string _key,T _t)
    {
        if(m_events.ContainsKey(_key))
        {
            Action<T> e = m_events[_key] as Action<T>;
            if(e!= null)
            {
                e(_t);
            }
            else
            {
                broadcastErr(_key);
            }
        }
    }

    public static void Broadcast<T,U>(string _key,T _t,U _u)
    {
        
        if(m_events.ContainsKey(_key))
        {
            Action<T,U> e = m_events[_key] as Action<T,U>;
            if(e!= null)
            {
                e(_t,_u);
            }
            else
            {
                broadcastErr(_key);
            }
        }
    }

    public static void Broadcast<T,U,I>(string _key,T _t,U _u,I _i)
    {
        
        if(m_events.ContainsKey(_key))
        {
            Action<T,U,I> e = m_events[_key] as Action<T,U,I>;
            if(e!= null)
            {
                e(_t,_u,_i);
            }
            else
            {
                broadcastErr(_key);
            }
        }
    }



    static bool equalsType(Delegate _e1, Delegate _e2)
    {
        return _e1.GetType()==_e2.GetType();
    }

    static void addEvent(string _key,Delegate _callBack)
    {
        m_events[_key]=_callBack;
    }

    static void removeEvent(string _key)
    {
        m_events.Remove(_key);
    }

    static void addErr(string _key)
    {
        Debug.LogError($"[Broadcaster.addErr]监听事件冲突:{_key}");
    }

    static void removeErr(string _key)
    {
        Debug.LogError($"[Broadcaster.removeErr]移除事件冲突:{_key}");
    }

    static void broadcastErr(string _key)
    {
        Debug.LogError($"[Broadcaster.broadcastErr]广播事件冲突:{_key}");
    }
}