using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using FGUFW.Core.System;

namespace GamePlay.GameLobby
{
    public class DefaultModule : IPlayModule
    {
        private bool _isInit;
        private GameLobbyPlayManager _playManager;

        public bool IsInit()
        {
            return _isInit;
        }

        public void OnInit(IPlayManager playManager)
        {
            _playManager = playManager as GameLobbyPlayManager;
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
