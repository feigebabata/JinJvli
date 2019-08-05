using UnityEngine;
using UnityEngine.UI;

namespace JinJvli
{
    public class Toast
    {
        public static class Config
        {
            public const float DEFAULT_DELAY=3;
        }
        Transform m_parent;

        public Toast()
        {
            m_parent = GameObject.Find("Canvas/Toasts").transform;
        }

        public void Show(string _text,float _delay)
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
            item.GetChild(0).GetComponent<Text>().text = _text;
            item.gameObject.SetActive(true);
            Coroutines.Inst.Delay(_delay,()=>{item.gameObject.SetActive(false);});
        }

        public void Clear()
        {
            foreach (Transform t in m_parent)
            {
                GameObject.Destroy(t.gameObject);
            }
        }
    }
}