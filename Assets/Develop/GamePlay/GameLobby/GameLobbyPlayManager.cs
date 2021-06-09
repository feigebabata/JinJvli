using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;

namespace GamePlay.GameLobby
{
    public class GameLobbyPlayManager : PlayManager
    {
        public override void Create()
        {
            base.Create();
            loadScene().Start();
        }
        
        public override void Destroy()
        {
            base.Destroy();
            
            
        }

        IEnumerator loadScene()
        {
            Debug.Log("Addressables.BuildPath:"+Addressables.BuildPath);
            Debug.Log("Addressables.RuntimePath:"+Addressables.RuntimePath);
            yield return Addressables.LoadSceneAsync("GamePlay.GameLobby");
            SceneLoading.I.Hide();

            Module<LobbyModule>().OnInit(this);
        }

    }
}
