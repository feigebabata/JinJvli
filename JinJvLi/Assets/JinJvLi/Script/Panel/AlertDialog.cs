using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JinJvli
{
    public class AlertDialog
    {
        Stack<GameObject> m_alertStack = new Stack<GameObject>();Transform m_parent;

        public AlertDialog()
        {
            m_parent = GameObject.Find("Canvas/AlertDialogs").transform;
        }

        public int Count
        {
            get
            {
                return m_alertStack.Count;
            }
        }

        public void Show(string _text,Action _ok,Action _cancel)
        {
            Transform item=null;
            foreach (Transform t in m_parent)
            {
                if(!t.gameObject.activeSelf)
                {
                    item = t;
                }
            }
            if(item == null)
            {
                item = GameObject.Instantiate(item,m_parent.GetChild(0));
            }
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = _text;
            item.GetComponent<Button>().onClick.RemoveAllListeners();
            item.GetComponent<Button>().onClick.AddListener(()=>
            {
                CloseCur();
                _cancel?.Invoke();
            });
            item.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            item.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(()=>
            {
                CloseCur();
                _ok?.Invoke();
            });
            item.gameObject.SetActive(true);
            m_alertStack.Push(item.gameObject);
        }

        public void CloseCur()
        {
            if(Count>0)
            {
                m_alertStack.Pop().SetActive(false);
            }
        }

        public void Clear()
        {
            foreach (Transform t in m_parent)
            {
                GameObject.Destroy(t.gameObject);
            }
            m_alertStack.Clear();
        }
    }
}