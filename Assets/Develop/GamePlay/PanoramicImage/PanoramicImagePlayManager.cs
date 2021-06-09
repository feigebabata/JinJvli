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
        }
        
        public override void Destroy()
        {
            base.Destroy();
            
        }

        IEnumerator loadScene()
        {
            yield return Addressables.LoadSceneAsync("GamePlay.PanoramicImage");
            SceneLoading.I.Hide();
            Module<PanoramicModule>().OnInit(this);
        }

    }
}
