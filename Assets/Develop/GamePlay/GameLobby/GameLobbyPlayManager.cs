using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;
using System;

namespace GamePlay.GameLobby
{
    public class GameLobbyPlayManager : PlayManager
    {
        public IMessenger<string,object> Messenger = new Messenger<string,object>();
        public override void Create()
        {
            base.Create();
            loadScene();

        }
        
        public override void Destroy()
        {
            base.Destroy();
            
            
        }

        async void loadScene()
        {
            Debug.Log("Addressables.BuildPath:"+Addressables.BuildPath);
            Debug.Log("Addressables.RuntimePath:"+Addressables.RuntimePath);
            await Addressables.LoadSceneAsync("GamePlay.GameLobby").Task;
            SceneLoading.I.Hide();

            Module<LobbyModule>().OnInit(this);
            Module<OnlineGameModule>().OnInit(this);
        }
    }
}
