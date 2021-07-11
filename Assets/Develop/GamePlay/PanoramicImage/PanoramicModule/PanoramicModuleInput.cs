using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.PanoramicImage
{
    public class PanoramicModuleInput : IDisposable
    {
        PanoramicImagePlayManager _playManager;
        public PanoramicModuleInput(PanoramicImagePlayManager playManager)
        {
            _playManager = playManager;
            
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    Camera.main.gameObject.AddComponent<GyroRotateCtrl>();
                }
                break;
                default:
                {
                    Camera.main.gameObject.AddComponent<MouseRotateCtrl>();
                }
                break;
            }
        }
        
        public void Dispose()
        {
            _playManager = null;
        }

    }
}
