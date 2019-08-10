using System;
using System.Collections;
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
        Toast m_toast = new Toast();
        AlertDialog m_alertDialog = new AlertDialog();

        GameObject m_loading;
        int m_showLoadingCount=0;
        Coroutine m_loadingAnim;

        public void Init()
        {
            m_panelParent = GameObject.Find("Canvas/Panels").transform;
            m_loading = GameObject.Find("Canvas").transform.Find("Loading").gameObject;
        }
        
        public void Clear()
        {
            m_panelConfigs.Clear();
            m_curPanel=null;
            m_panelConfigs.Clear();
            for (int i = 0; i < m_panels.Count; i++)
            {
                m_panels[i].OnHide();
                m_panels[i].OnClose();
                GameObject.Destroy(m_panels[i].gameObject);
            }
            m_panels.Clear();
            m_toast.Clear();
            m_alertDialog.Clear();
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(m_showLoadingCount==0)
                {
                    if(m_alertDialog.Count>0)
                    {
                        m_alertDialog.CloseCur();
                        return;
                    }
                    m_curPanel?.OnClickClose();
                }
            }
        }

        /// <summary>
        /// 别在Open里打开新界面 会出Bug
        /// </summary>
        /// <param name="_createData"></param>
        /// <typeparam name="T"></typeparam>
        public void Open<T>(object _createData=null) where T:PanelBase
        {
            PanelBase panel=null;
            Type panelType = null;
            // //隐藏旧界面
            if(m_curPanel)
            {
                m_curPanel.OnHide();
            }

            //打开新界面
            panelType = typeof(T);
            var panelConfig = getPanelConfig(panelType);
            for (int i = m_panels.Count-1; i >=0 ; i--)
            {
                if(m_panels[i] is T && !m_panels[i].IsShow)
                {
                    panel = m_panels[i];
                    break;
                }
            }
            if(panel == null)
            {
                panel=createPanel(panelConfig.PrefabPath,panelType);
                panel.OnCreate(_createData);
            }
            
            addPanelStack(panelType);
            m_curPanel = panel;
            panel.OnShow();
        }
        public void CloseCurPanel()
        {
            if(m_panelStack.Count>1)
            {
                Type panelType = removePanelStack();
                PanelBase panel=m_curPanel;
                var panelConfig = getPanelConfig(panelType);
                panel.OnHide();
                panel.OnClose();
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
                    if(m_panels[i].GetType()==panelType && m_panels[i] != m_curPanel)
                    { 
                        panel= m_panels[i];
                        break;
                    }
                }
                if(panel == null)
                {
                    panel = createPanel(panelConfig.PrefabPath,panelType);
                    panel.OnCreate();
                }
                m_curPanel = panel;
                panel.OnShow();
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
                    if(atts[i] is PanelConfigAttribute)
                    {
                        PanelConfigAttribute panelConfig = atts[i] as PanelConfigAttribute;
                        m_panelConfigs.Add(_panelType,panelConfig);
                        return panelConfig;
                    }
                }
            }
            return null;
        }

        public void ShowToast(string _text,float _delay=Toast.Config.DEFAULT_DELAY)
        {
            m_toast.Show(_text,_delay);
        }

        public void ShowAlertDialog(string _text,Action _ok,Action _cancel=null)
        {
            m_alertDialog.Show(_text,_ok,_cancel);
        }

        public void ShowLoading()
        {
            m_showLoadingCount++;
            if(m_showLoadingCount==1)
            {
                m_loading.SetActive(true);
                m_loadingAnim = loadingAnim().Start();
            }
        }

        IEnumerator loadingAnim()
        {
            Transform loading_logo = m_loading.transform.GetChild(0);
            var wait = new WaitForEndOfFrame();
            while (true)
            {
                loading_logo.Rotate(0,0,-Time.deltaTime*90);
                yield return wait;
            }
        }

        public void HideLoading()
        {
            if(m_showLoadingCount==0)
            {
                return;
            }
            m_showLoadingCount--;
            if(m_showLoadingCount==0)
            {
                m_loading.SetActive(false);
                m_loadingAnim.Stop();
            }
        }
    }
    
    /// <summary>
    /// 表示这是一个UI界面
    /// </summary>
    public class PanelBase : MonoBehaviour
    {
        [NonSerialized]
        public bool IsShow;
        public virtual void OnCreate(object _createData = null){}
        public virtual void OnClose(){}
        public virtual void OnShow(){IsShow=true;gameObject.SetActive(true);}
        public virtual void OnHide(){IsShow=false;gameObject.SetActive(false);}
        public virtual void OnClickClose(){Main.Manager<PanelManager>().CloseCurPanel();}
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
        public PanelConfigAttribute(string _prefabPath,bool _autoDestroy=false)
        {
            PrefabPath = _prefabPath;
            AutoDestroy = _autoDestroy;
        }
        
    }
}