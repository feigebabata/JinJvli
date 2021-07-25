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
        // public INetworkSyncSystem NetworkSyncSystem;
        public PB_GameStart GameStart{get; private set;}
        public PB_Player SelfInfo{get; private set;}
        public StepGridConfig StepGridConfig;
        public IFrameSyncSystem FrameSyncSystem;

        public override void Create(params object[] datas)
        {
            base.Create(datas);

            GameStart = datas[0] as PB_GameStart;
            GamePlayID=GameStart.GamePlayID;
            var playerInfo = datas[1] as PB_PlayerInfo;

            foreach (var player in GameStart.Players)
            {
                if(player.PlayerInfo.ID==playerInfo.ID)
                {
                    SelfInfo = player;
                    break;
                }
            }

            ScreenHelper.Portrait();
            // NetworkSyncSystem = new NetworkSyncSystem();
            // NetworkSyncSystem.OnInit(GamePlayID,SelfInfo.PlaceIndex);
            // NetworkSyncSystem.OnEnable();

            FrameSyncSystem = new FrameSyncSystem();
            FrameSyncSystem.OnInit(GameStart.Players.Count,SelfInfo.PlaceIndex,GamePlayID);

            loadScene();
        }

        public override void Destroy()
        {
            base.Destroy();
            
            FrameSyncSystem.OnDisable();
            FrameSyncSystem.OnRelease();
            FrameSyncSystem=null;

            ScreenHelper.Landscape();
        }

        async void loadScene()
        {
            await Addressables.LoadSceneAsync("GamePlay.StepGrid").Task;
            StepGridConfig = await Addressables.LoadAssetAsync<StepGridConfig>("GamePlay.StepGrid.StepGridConfig").Task;
            Debug.Log(Screen.orientation);

            Module<DefaultModule>().OnEnable();
        }



    }
}
