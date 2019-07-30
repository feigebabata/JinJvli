using System;
using System.Collections.Generic;
using JinJvLi;
using UnityEngine;

namespace JinJvli
{
    public class PanelManager : IManager
    {
        List<PanelBase> m_panels = new List<PanelBase>();
        Stack<Type> m_panelStack = new Stack<Type>();
        Dictionary<Type,PanelConfigAttribute> m_panelConfigs = new Dictionary<Type, PanelConfigAttribute>();
        Transform m_panelParent;
        PanelBase m_curPanel;

        public void Init()
        {
            m_panelParent = GameObject.Find("Canvas").transform;
        }
        
        public void Clear(){}

        public void Update(){}

        /// <summary>
        /// 别在Open里打开新界面 会出Bug
        /// </summary>
        /// <param name="_openData"></param>
        /// <typeparam name="T"></typeparam>
        public void Open<T>(object _openData=null) where T:PanelBase
        {
            PanelBase panel=null;
            Type panelType = null;
            // //隐藏旧界面
            if(m_curPanel)
            {
                m_curPanel.Hide();
            }

            //打开新界面
            panelType = typeof(T);
            var panelConfig = getPanelConfig(panelType);
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
                panel=createPanel(panelConfig.PrefabPath,panelType);
            }
            else
            {
                if(!panelConfig.Only)
                {
                    // panel =;
                }
            }
            addPanelStack(panelType);
            m_curPanel = panel;
            panel.Open(_openData);
            panel.Show();
        }
        public void CloseCurPanel()
        {
            if(m_panelStack.Count>1)
            {
                Type panelType = removePanelStack();
                PanelBase panel=m_curPanel;
                var panelConfig = getPanelConfig(panelType);
                panel.Hide();
                panel.Close();
                if (panelConfig.AutoDestroy)
                {
                    m_panels.Remove(panel);
                    GameObject.Destroy(panel.gameObject);
                }

                //显示之前界面
                panel=null;
                panelType = getPanelStack();
                panelConfig = getPanelConfig(panelType);
                for (int i = m_panels.Count-1; i >= 0; i--)
                {
                    if(m_panels[i].GetType()==panelType)
                    { 
                        panel= m_panels[i];
                        break;
                    }
                }
                if(panel == null)
                {
                    panel = createPanel(panelConfig.PrefabPath,panelType);
                }
                m_curPanel = panel;
                panel.Open();
                panel.Show();
            }
            else
            {
                Debug.LogWarning($"[CloseCurPanel]最后一个界面了");
            }
        }

        void addPanel(PanelBase _panel)
        {
            m_panels.Add(_panel);
        }

        void addPanelStack(Type _type)
        {
            m_panelStack.Push(_type);
        }

        Type removePanelStack()
        {
            return m_panelStack.Pop();
        }

        Type getPanelStack()
        {
            return m_panelStack.Peek();
        }

        PanelBase createPanel(string _prefabPath,Type _panelType)
        {
            PanelBase panel = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(_prefabPath),m_panelParent).GetComponent<PanelBase>();
            panel.name=_panelType.Name;
            panel.transform.localPosition = Vector3.zero;
            addPanel(panel);
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
    
    /// <summary>
    /// 表示这是一个UI界面
    /// </summary>
    public class PanelBase : MonoBehaviour
    {
        public bool IsShow;
        public virtual void Open(object _openData = null){}
        public virtual void Close(){}
        public virtual void Show(){IsShow=true;gameObject.SetActive(true);}
        public virtual void Hide(){IsShow=false;gameObject.SetActive(false);}
    }

    public class PanelConfigAttribute : Attribute
    {
        /// <summary>
        /// 界面预制体路径
        /// </summary>
        /// <value></value>
        public string PrefabPath{ get;private set;}
        /// <summary>
        /// 关闭后自动销毁
        /// </summary>
        /// <value></value>
        public bool AutoDestroy{ get;private set;}
        /// <summary>
        /// 只存在一个
        /// </summary>
        /// <value></value>
        public bool Only{ get;private set;}
        public PanelConfigAttribute(string _prefabPath,bool _autoDestroy=false,bool _only=true)
        {
            PrefabPath = _prefabPath;
            AutoDestroy = _autoDestroy;
            Only = _only;
        }
        
    }
}