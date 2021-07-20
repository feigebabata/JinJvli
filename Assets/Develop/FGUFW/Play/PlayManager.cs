using System;
using System.Collections;
using System.Collections.Generic;
using FGUFW.Core;
using UnityEngine;

namespace FGUFW.Play
{
    public abstract class PlayManager
    {

        private Dictionary<Type,IPlayModule> _modelDict = new Dictionary<Type, IPlayModule>();

        public U Module<U>() where U : PlayModule
        {
            Type type = typeof(U);
            if(!_modelDict.ContainsKey(type))
            {
                PlayModule client = Activator.CreateInstance(type,this) as PlayModule;
                _modelDict[type]=client;
            }
            return (U)_modelDict[type];
        }
		
        public virtual void Destroy()
        {
            foreach (var item in _modelDict)
            {
                item.Value.OnDisable();
                item.Value.Dispose();
            }
            _modelDict.Clear();
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