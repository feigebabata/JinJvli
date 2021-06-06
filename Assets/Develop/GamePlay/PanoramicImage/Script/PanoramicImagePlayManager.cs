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
        public override void Create()
        {
            base.Create();

            loadScene().Start();
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        public override void Destroy()
        {
            Cursor.lockState = CursorLockMode.None;
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);

            base.Destroy();
            
        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.PanoramicImage");
            SceneLoading.I.Hide();
            
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
        }

        private void onClickBack(object data)
        {
            Destroy();
            new GameLobby.GameLobbyPlayManager().Create();
        }

    }
}
