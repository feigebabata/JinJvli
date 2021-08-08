using System;
using System.Collections;
using System.Collections.Generic;
using FGUFW.Core;
using UnityEngine;

namespace FGUFW.Play
{
    public abstract class WorldBase
    {

        private Dictionary<Type,IPart> _partDic = new Dictionary<Type, IPart>();

        public U Part<U>() where U : PartBase
        {
            Type type = typeof(U);
            if(!_partDic.ContainsKey(type))
            {
                PartBase client = Activator.CreateInstance(type,this) as PartBase;
                _partDic[type]=client;
            }
            return (U)_partDic[type];
        }
		
        public virtual void Destroy()
        {
            foreach (var item in _partDic)
            {
                item.Value.OnDisable();
                item.Value.Dispose();
            }
            _partDic.Clear();
            GlobalMessenger.M.Remove(GlobalMsgID.OnApplicationQuit,onApplicationQuit);
        }

        public virtual void Create(params object[] datas)
        {
            SceneLoading.I.Show();
            GlobalMessenger.M.Add(GlobalMsgID.OnApplicationQuit,onApplicationQuit);
        }

        public long GamePlayID{get;protected set;}

        private void onApplicationQuit(object obj)
        {
            Destroy();
        }

    }
}