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
        public IPlayerInput PlayerInput;
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
            
            MonoBehaviourEvent.I.LateUpdateListener -= LateUpdate;
            PlayerInput.OnClickBack -= onClickBack;
            
            PlayerInput.OnRelease();
            PlayerInput=null;
        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.StepGrid");
            SceneLoading.I.Hide();
            Debug.Log(Screen.orientation);
            
            #if UNITY_ANDROID// && !UNITY_EDITOR
            PlayerInput = new AndroidPlayerInput();
            #else
            PlayerInput = new PCPlayerInput();
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
