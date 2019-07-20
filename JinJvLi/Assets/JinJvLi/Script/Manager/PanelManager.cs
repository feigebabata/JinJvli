using System;
using System.Collections.Generic;
using JinJvLi;
using UnityEngine;

namespace JinJvli
{
    public class PanelManager : IManager
    {
        List<PanelBase> m_panels = new List<PanelBase>();
        Queue<Type> m_panelQueue = new Queue<Type>();
        Dictionary<Type,PanelConfigAttribute> m_panelConfigs = new Dictionary<Type, PanelConfigAttribute>();
        Transform m_panelParent;

        public void Init()
        {
            m_panelParent = GameObject.Find("Canvas").transform;
        }
        
        public void Clear(){}

        public void Update(){}

        public void Open<T>(object _openData=null) where T:PanelBase
        {
            Type panelType = typeof(T);
            var panelConfig = getPanelConfig(panelType);
            PanelBase panel=null;
            for (int i = m_panels.Count-1; i >=0 ; i--)
            {
                if(m_panels[i].GetType()==panelType)
                {
                    panel = m_panels[i];
                    break;
                }
            }
            if(panel== null)
            {
                createPanel(panelConfig.PrefabPath,panelType).Open(_openData);
            }
            else
            {
                if(panelConfig.Only)
                {
                    panel.Open(_openData);
                }
                else
                {

                }
            }
        }
        public void CloseCurPanel()
        {
            if(m_panelQueue.Count>0)
            {
                Type panelType = m_panelQueue.Dequeue();
                PanelBase panel=null;
                var panelConfig = getPanelConfig(panelType);
                for (int i = m_panels.Count-1; i >= 0; i--)
                {
                    if(m_panels[i].GetType()==panelType)
                    { 
                        panel= m_panels[i];
                        break;
                    }
                }
                if(panel != null)
                {
                    panel.Close();
                    if (panelConfig.AutoDestroy)
                    {
                        m_panels.Remove(panel);
                        panel=null;
                    }
                }
                else
                {
                    Debug.LogError($"[CloseCurPanel]找不到当前界面{panelType}");
                    return;
                }

                //显示之前界面
                if(m_panelQueue.Count>0)
                {
                    panel=null;
                    panelType = m_panelQueue.Peek();
                    panelConfig = getPanelConfig(panelType);
                    for (int i = m_panels.Count-1; i >= 0; i--)
                    {
                        if(m_panels[i].GetType()==panelType)
                        { 
                            panel= m_panels[i];
                            break;
                        }
                    }
                    if(panel != null)
                    {
                        panel.Open();
                    }
                    else
                    {
                        panel = createPanel(panelConfig.PrefabPath,panelType);
                    }
                }
            }
        }

        void addPanel(PanelBase _panel)
        {
            m_panels.Add(_panel);
        }

        void addPanelQueue(Type _type)
        {
            m_panelQueue.Enqueue(_type);
        }

        PanelBase createPanel(string _prefabPath,Type _panelType)
        {
            PanelBase panel = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(_prefabPath),m_panelParent).GetComponent<PanelBase>();
            panel.name=_panelType.Name;
            panel.transform.localPosition = Vector3.zero;
            addPanel(panel);
            addPanelQueue(_panelType);
            return panel;
        }

        PanelConfigAttribute getPanelConfig(Type _panelType)
        {
            if(m_panelConfigs.ContainsKey(_panelType))
            {
                return m_panelConfigs[_panelType];
            }
            else
            {
                var atts = _panelType.GetCustomAttributes(false);
                for (int i = 0; i < atts.Length; i++)
                {
                    if(atts[i].GetType()==typeof(PanelConfigAttribute))
                    {
                        PanelConfigAttribute panelConfig = atts[i] as PanelConfigAttribute;
                        m_panelConfigs.Add(_panelType,panelConfig);
                        return panelConfig;
                    }
                }
            }
            return null;
        }
    }
}