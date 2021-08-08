using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FGUFW.Core
{
    public class GlobalAppEventSystem : MonoSingleton<GlobalAppEventSystem>
    {
    
        public Action UpdateListener;
        public Action LateUpdateListener;


        protected override bool IsDontDestroyOnLoad()
        {
            return true;
        }
        

        void Update()
        {
            UpdateListener?.Invoke();
        }

        void LateUpdate()
        {
            LateUpdateListener?.Invoke();
        }
    }
}
