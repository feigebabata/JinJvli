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
        public INetworkSyncSystem NetworkSyncSystem;

        public override void Create()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            base.Create();

            NetworkSyncSystem = new NetworkSyncSystem();
            NetworkSyncSystem.OnInit();
            NetworkSyncSystem.OnEnable();

            MonoBehaviourEvent.I.UpdateListener+=Update;

            loadScene().Start();
        }

        public override void Destroy()
        {
            base.Destroy();

            MonoBehaviourEvent.I.UpdateListener-=Update;
            NetworkSyncSystem.OnRelease();
            NetworkSyncSystem=null;

            Screen.orientation = ScreenOrientation.Landscape;
        }

        private void Update()
        {
            NetworkSyncSystem.SendMsg(null,(uint)UnityEngine.Random.Range(1,int.MaxValue));
        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.StepGrid");
            Debug.Log(Screen.orientation);

            Module<DefaultModule>().OnInit(this);
            Module<DefaultModule>().OnShow();
        }


    }
}
