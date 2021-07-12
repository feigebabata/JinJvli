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
            NetworkSyncSystem = new NetworkSyncSystem();
            NetworkSyncSystem.OnInit(GamePlayID);
            NetworkSyncSystem.OnEnable();


            loadScene();

            testCor = test().Start();
        }

        public override void Destroy()
        {
            base.Destroy();

            NetworkSyncSystem.OnRelease();
            NetworkSyncSystem=null;

            ScreenHelper.Landscape();
            testCor.Stop();
        }

        async void loadScene()
        {
            await Addressables.LoadSceneAsync("GamePlay.StepGrid").Task;
            Debug.Log(Screen.orientation);

            Module<DefaultModule>().OnInit(this);
            Module<DefaultModule>().OnShow();
        }

        Coroutine testCor;
        IEnumerator test()
        {
            while (true)
            {
                yield return null;
                NetworkSyncSystem.SendMsg((uint)UnityEngine.Random.Range(1,int.MaxValue),1,null);
            }
        }


    }
}
