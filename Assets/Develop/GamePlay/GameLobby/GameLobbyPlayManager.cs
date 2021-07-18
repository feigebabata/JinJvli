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
        public GameItemDatas GameDatas;

        public override void Create(params object[] datas)
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
            GameDatas = await Addressables.LoadAssetAsync<GameItemDatas>("GamePlay.GameLobby.GameDatas").Task;
            SceneLoading.I.Hide();

            Module<LobbyModule>().OnEnable();
            Module<OnlineGameModule>();
        }
    }
}
