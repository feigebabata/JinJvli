using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.StepGrid
{
    public class DefaultModuleInput : IDisposable
    {
        StepGridPlayManager _playManager;
        public DefaultModuleInput(StepGridPlayManager playManager)
        {
            _playManager = playManager;
        }
        
        public void Dispose()
        {
            _playManager = null;
        }

    }
}
