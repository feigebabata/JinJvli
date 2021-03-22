using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;
using System;

namespace GamePlay.PanoramicImage
{
    public class PanoramicImagePlayManager : PlayManager
    {
        public IPlayerInput PlayerInput;
        public override void Create()
        {
            base.Create();

            loadScene().Start();
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public override void Destroy()
        {
            base.Destroy();
            Cursor.lockState = CursorLockMode.None;
            
            MonoBehaviourEvent.I.LateUpdateListener -= LateUpdate;
            PlayerInput.OnClickBack -= onClickBack;
            
            PlayerInput.OnRelease();
            PlayerInput=null;
            
        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.PanoramicImage");
            SceneLoading.I.Hide();
            #if UNITY_ANDROID && !UNITY_EDITOR
            PlayerInput = new AndroidPlayerInput();
            GameObject.Find("Main Camera").AddComponent<AndroidCameraRotateCtrl>().PlayerInput = PlayerInput as AndroidPlayerInput;
            #else
            PlayerInput = new PCPlayerInput();
            GameObject.Find("Main Camera").AddComponent<PCCameraRotateCtrl>();
            #endif
            PlayerInput.OnInit();

            MonoBehaviourEvent.I.LateUpdateListener += LateUpdate;
            
            PlayerInput.OnClickBack += onClickBack;
        }

        private void onClickBack()
        {
            Destroy();
            new GameLobby.GameLobbyPlayManager().Create();
        }

        void LateUpdate()
        {
            PlayerInput.LateUpdate();
        }

    }
}
