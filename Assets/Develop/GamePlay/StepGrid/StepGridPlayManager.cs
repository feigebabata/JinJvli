using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;
using System;

namespace GamePlay.StepGrid
{
    public class StepGridPlayManager : PlayManager
    {
        public IMessenger<StepGridMsgID,object> Messenger = new Messenger<StepGridMsgID,object>();
        public INetworkSyncSystem NetworkSyncSystem;

        public override void Create()
        {
            ScreenHelper.Portrait();
            base.Create();
            GamePlayID=12;
            NetworkSyncSystem = new NetworkSyncSystem();
            NetworkSyncSystem.OnInit(GamePlayID,0);
            NetworkSyncSystem.OnEnable();


            loadScene();
        }

        public override void Destroy()
        {
            base.Destroy();

            NetworkSyncSystem.OnRelease();
            NetworkSyncSystem=null;

            ScreenHelper.Landscape();
        }

        async void loadScene()
        {
            await Addressables.LoadSceneAsync("GamePlay.StepGrid").Task;
            Debug.Log(Screen.orientation);

            Module<DefaultModule>().OnEnable();
        }


    }
}
