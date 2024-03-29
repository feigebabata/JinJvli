using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class MonoBehaviourEvent : MonoSingleton<MonoBehaviourEvent>
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
