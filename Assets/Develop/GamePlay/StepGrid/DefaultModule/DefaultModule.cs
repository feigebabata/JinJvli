using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.StepGrid
{
    public class DefaultModule : IPlayModule
    {
        private bool _isInit;
        private StepGridPlayManager _playManager;

        public bool IsInit()
        {
            return _isInit;
        }

        public void OnInit(IPlayManager playManager)
        {
            if(!_isInit)
            {
                _isInit = true;
                _playManager = playManager as StepGridPlayManager;
            }
        }

        public void OnRelease()
        {
            if(_isInit)
            {
                _isInit = false;
                _playManager = null;

            }
        }

        public void OnShow()
        {
            if(_isInit)
            {

            }
        }

        public void OnHide()
        {
            if(_isInit)
            {

            }
        }

    }
}
