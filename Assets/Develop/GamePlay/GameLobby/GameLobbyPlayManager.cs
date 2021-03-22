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
        public IPlayerInput PlayerInput;
        public override void Create()
        {
            base.Create();
            loadScene().Start();
        }
        
        public override void Destroy()
        {
            base.Destroy();
            
            MonoBehaviourEvent.I.LateUpdateListener -= LateUpdate;
            
            PlayerInput.OnRelease();
            PlayerInput=null;
        }

        IEnumerator loadScene()
        {
            Debug.Log("Addressables.BuildPath:"+Addressables.BuildPath);
            yield return Addressables.LoadSceneAsync("GamePlay.GameLobby");
            SceneLoading.I.Hide();
            #if UNITY_ANDROID && !UNITY_EDITOR
            PlayerInput = new AndroidPlayerInput();
            GameObject.Find("character/Main Camera").AddComponent<AndroidCameraRotateCtrl>().enabled=false;
            #else
            PlayerInput = new PCPlayerInput();
            GameObject.Find("character/Main Camera").AddComponent<PCCameraRotateCtrl>().enabled=false;
            #endif
            PlayerInput.OnInit();

            MonoBehaviourEvent.I.LateUpdateListener += LateUpdate;

            Module<DefaultModule>().OnInit(this);
        }

        void LateUpdate()
        {
            PlayerInput.LateUpdate();
        }

    }
}
