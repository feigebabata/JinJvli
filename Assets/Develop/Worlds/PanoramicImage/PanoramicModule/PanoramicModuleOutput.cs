using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.PanoramicImage
{
    public class PanoramicModuleOutput : IDisposable
    {
        PanoramicImagePlayManager _playManager;
        public PanoramicModuleOutput(PanoramicImagePlayManager playManager)
        {
            _playManager = playManager;
        }
        
        public void Dispose()
        {
            _playManager = null;
        }

    }
}
