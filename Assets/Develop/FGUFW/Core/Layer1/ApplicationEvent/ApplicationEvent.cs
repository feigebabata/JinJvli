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
    }
}
