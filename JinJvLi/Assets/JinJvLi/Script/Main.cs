using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using JinJvLi;

namespace JinJvli
{   
    public class Main:MonoSingleton<Main>
    {
        Dictionary<Type,IManager> m_managers = new Dictionary<Type,IManager>();
        Dictionary<Type,IManager>.Enumerator m_mngs;

        void Start()
        {
            setAppConfig();
            init();
        }
        
        void OnApplicationQuit()
        {
            clearAllManager();
        }

        void setAppConfig()
        {
            Application.targetFrameRate=30;
            // Debug.unityLogger.logEnabled=false;
        }

        void init()
        {
            createAllManager();
            initAllManager();
        }

        void initEnd()
        {
            Manager<PanelManager>().Open<LoginPanel>();
        }

        void Update()
        {
            updateAllManager();
        }

        public void ReStart()
        {
            Coroutines.Inst.StopAllCoroutines();
            Broadcaster.ClearAll();
            clearAllManager();
            initAllManager();
        }

        public static T Manager<T>() where T : IManager
        {
            IManager mng;
            if(Inst.m_managers.TryGetValue(typeof(T),out mng))
            {
                return (T)mng;
            }
            return default(T);
        }

        /// <summary>
        /// 使用反射创建所有实现IManager的类
        /// </summary>
        void createAllManager()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            Type imsgType = typeof(IManager);
            for (int i = 0; i < types.Length; i++)
            {
                if(new List<Type>(types[i].GetInterfaces()).Contains(imsgType))
                {
                    IManager mng = Activator.CreateInstance(types[i]) as IManager;
                    m_managers.Add(types[i],mng);
                }
            }
            m_mngs = m_managers.GetEnumerator();
        }

        void initAllManager()
        {
            m_mngs = m_managers.GetEnumerator();
            while (m_mngs.MoveNext())
            {
                m_mngs.Current.Value.Init();
            }
            initEnd();
        }

        void updateAllManager()
        {
            m_mngs = m_managers.GetEnumerator();
            while (m_mngs.MoveNext())
            {
                m_mngs.Current.Value.Update();
            }
        }

        void clearAllManager()
        {
            m_mngs = m_managers.GetEnumerator();
            while (m_mngs.MoveNext())
            {
                m_mngs.Current.Value.Clear();
            }
        }
    }
}