using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;

namespace GamePlay.StepGrid
{
    public class StepGridPlayManager : PlayManager
    {
        public override void Create()
        {
            base.Create();
            Screen.orientation = ScreenOrientation.Portrait;
            loadScene().Start();
        }
        
        public override void Destroy()
        {
            base.Destroy();
            Screen.orientation = ScreenOrientation.Landscape;
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);

        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.StepGrid");
            SceneLoading.I.Hide();
            Debug.Log(Screen.orientation);

            
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
        }

        private void onClickBack(object data)
        {
            Destroy();
            new GameLobby.GameLobbyPlayManager().Create();
        }


    }
}
