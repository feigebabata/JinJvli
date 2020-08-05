using System;
using System.Collections;
using System.Collections.Generic;
using FGUFW.Core;

namespace FGUFW.Play
{
    abstract public class PlayManager<T> : IPlayManager
    {
		private static T _instance;
		public static T I
		{
			get
			{
				if (PlayManager<T>._instance == null)
				{
					PlayManager<T>._instance = Activator.CreateInstance<T>();
					if (PlayManager<T>._instance != null)
					{
						(PlayManager<T>._instance as PlayManager<T>).Init();
					}
				}

				return PlayManager<T>._instance;
			}
		}

        Dictionary<Type,IPlayModel> _modelDict = new Dictionary<Type, IPlayModel>(); 

        public U Model<U>() where U : IPlayModel
        {
            Type type = typeof(U);
            if(!_modelDict.ContainsKey(type))
            {
                IPlayModel client = Activator.CreateInstance(type) as IPlayModel;
                _modelDict[type]=client;
            }
            return (U)_modelDict[type];
        }

        public void Release()
        {
            foreach (var model in _modelDict)
            {
                model.Value.Release();
            }
            _modelDict.Clear();
        }

		public virtual void Init()
		{

		}
    }
}