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
        private ushort _gameplayID = ushort.MaxValue;

        public U Module<U>() where U : IPlayModule
        {
            Type type = typeof(U);
            if(!_modelDict.ContainsKey(type))
            {
                IPlayModule client = Activator.CreateInstance(type) as IPlayModule;
                _modelDict[type]=client;
            }
            return (U)_modelDict[type];
        }
		
        public virtual void Destroy()
        {
            foreach (var item in _modelDict)
            {
                item.Value.OnRelease();
            }
            _modelDict.Clear();
        }

        public virtual void Create()
        {
            SceneLoading.I.Show();
        }

        public ushort GamePlayID
        {
            get
            {
                if(_gameplayID==ushort.MaxValue)
                {
                    _gameplayID = GamePlay.GameLobby.GameLobbyPlayManager.GetGamePlayID(this.GetType().FullName);
                }
                return _gameplayID;
            }
        }
    }
}