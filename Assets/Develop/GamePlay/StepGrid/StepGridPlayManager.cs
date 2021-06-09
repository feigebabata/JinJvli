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
            Screen.orientation = ScreenOrientation.Portrait;
            base.Create();
            loadScene().Start();
        }
        
        public override void Destroy()
        {
            base.Destroy();
            Screen.orientation = ScreenOrientation.Landscape;

        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.StepGrid");
            SceneLoading.I.Hide();
            Debug.Log(Screen.orientation);

            Module<DefaultModule>().OnInit(this);
        }


    }
}
