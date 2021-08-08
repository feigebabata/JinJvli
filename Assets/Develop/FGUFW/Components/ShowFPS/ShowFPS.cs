using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Play
{
    public class ShowFPS : MonoBehaviour
    {
        public GUIStyle UIStyle;

        int _fps;
        string _fpsText="";
        int _seconds;
        void OnGUI()
        {
            GUILayout.Label(_fpsText,UIStyle);
        }

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if((int)Time.time > _seconds)
            {
                _seconds = (int)Time.time;
                _fpsText = _fps.ToString();
                _fps=0;
            }
            _fps++;
        }
    }
}
