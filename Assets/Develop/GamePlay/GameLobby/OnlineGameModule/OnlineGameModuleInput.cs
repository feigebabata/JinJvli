using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.GameLobby
{
    public class OnlineGameModuleInput : IDisposable
    {
        GameLobbyPlayManager _playManager;
        public OnlineGameModuleInput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
        }
        
        public void Dispose()
        {
            _playManager = null;
        }

    }
}
