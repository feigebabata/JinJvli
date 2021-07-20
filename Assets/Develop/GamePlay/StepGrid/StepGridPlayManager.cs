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
        public PB_GameStart GameStart{get; private set;}
        public PB_Player SelfInfo{get; private set;}

        public override void Create(params object[] datas)
        {
            base.Create(datas);

            GameStart = datas[0] as PB_GameStart;
            GamePlayID=GameStart.GamePlayID;
            foreach (var item in GameStart.Players)
            {
                if(item.PlayerInfo.ID==SystemInfo.deviceUniqueIdentifier)
                {
                    SelfInfo = item;
                    break;
                }
            }

            ScreenHelper.Portrait();
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
