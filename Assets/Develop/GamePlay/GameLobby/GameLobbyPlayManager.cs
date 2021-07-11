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
        static public GameItemDatas GameItemDatas;

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
            GameItemDatas = await Addressables.LoadAssetAsync<GameItemDatas>("GamePlay.GameLobby.GameDatas").Task;
            SceneLoading.I.Hide();

            Module<LobbyModule>().OnInit(this);
        }

        public static ushort GamePlayID(string gameplayName)
        {
            for (int i = 0; i < GameItemDatas.Datas.Length; i++)
            {
                if(GameItemDatas.Datas[i].TypeName==gameplayName)
                {
                    return (ushort)(i+1);
                }
            }
            return 0;
        }

        public static string GamePlayName(ushort gameplayID)
        {
            int index = gameplayID-1;
            if(index<GameItemDatas.Datas.Length)
            {
                return GameItemDatas.Datas[index].TypeName;
            }
            return typeof(GameLobbyPlayManager).FullName;
        }

    }
}
