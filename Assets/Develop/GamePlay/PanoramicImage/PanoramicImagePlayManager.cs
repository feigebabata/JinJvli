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

            loadScene();
        }
        
        public override void Destroy()
        {
            base.Destroy();
            
        }

        async void loadScene()
        {
            await Addressables.LoadSceneAsync("GamePlay.PanoramicImage").Task;
            SceneLoading.I.Hide();
            Module<PanoramicModule>().OnInit(this);
        }

    }
}
