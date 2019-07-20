using UnityEngine;

public class MonoSingleton<T>:MonoBehaviour where T : MonoBehaviour 
{
    static T m_instance;
    public static T Inst
    {
        get
        {
            if(m_instance==null)
            {
                m_instance = GameObject.FindObjectOfType<T>();
                if(!m_instance)
                {
                    m_instance=new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return m_instance;
        }
    }
}