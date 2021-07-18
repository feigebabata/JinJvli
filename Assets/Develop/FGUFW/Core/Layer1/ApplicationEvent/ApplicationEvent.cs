using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGUFW.Core
{
    public class ApplicationEvent : MonoSingleton<ApplicationEvent>
    {
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                GlobalMessenger.M.Broadcast(GlobalMsgID.OnBackKey,null);
            }
        }

        /// <summary>
        /// Callback sent to all game objects before the application is quit.
        /// </summary>
        void OnApplicationQuit()
        {
            GlobalMessenger.M.Broadcast(GlobalMsgID.OnApplicationQuit,null);
        }
    }
}
