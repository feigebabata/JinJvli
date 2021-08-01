using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;

namespace FGUFW.Core
{
    public class AndroidBehaviour : MonoSingleton<AndroidBehaviour>
    {

        AndroidJavaObject activity;


        protected override bool IsDontDestroyOnLoad()
        {
            return true;
        }
            
        protected override void Init()
        {
            base.Init();
            #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass ja = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            activity = ja.GetStatic<AndroidJavaObject>("currentActivity");
            #endif
        }

        public void LockAcquire()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            activity.Call("lockAcquire");
            #endif
        }

        public void LockRelease()
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            activity.Call("lockRelease");
            #endif
        }

        void OnAndroidMsg(string msg)
        {
            Debug.Log(msg);
        }
        
    }
}
