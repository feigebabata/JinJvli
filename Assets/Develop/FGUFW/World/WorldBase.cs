using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FGUFW.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FGUFW.Play
{
    public abstract class WorldBase
    {
        public static WorldBase Current{get;private set;}
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
            Current=null;
        }

        public virtual void Create(params object[] datas)
        {
            Current = this;
        }

        public long GamePlayID{get;protected set;}

        private void onApplicationQuit(object obj)
        {
            Destroy();
        }

        protected Task LoadScene(string path)
        {
            return Addressables.LoadSceneAsync(path).Task;
        }

        protected void logicUpdate()
        {
            foreach (var item in _partDic)
            {
                item.Value.LogicUpdate();
            }
        }

    }
}