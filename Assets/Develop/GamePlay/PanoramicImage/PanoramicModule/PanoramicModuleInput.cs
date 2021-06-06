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
        }
        
        public void Dispose()
        {
            _playManager = null;
        }

    }
}
