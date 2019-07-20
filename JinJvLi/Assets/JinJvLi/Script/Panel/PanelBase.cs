using System;
using UnityEngine;

namespace JinJvLi
{
    /// <summary>
    /// 表示这是一个UI界面
    /// </summary>
    public class PanelBase : MonoBehaviour
    {
        public virtual void Open(object _openData = null){}
        public virtual void Close(){}
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