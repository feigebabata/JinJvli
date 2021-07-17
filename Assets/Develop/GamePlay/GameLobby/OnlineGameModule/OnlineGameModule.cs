using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;

namespace GamePlay.GameLobby
{
    public class  OnlineGameModule : PlayModule<GameLobbyPlayManager>
    {
        private  OnlineGameModuleInput _moduleInput;
        private  OnlineGameModuleOutput _moduleOutput;

        public OnlineGameModule(PlayManager playManager) : base(playManager)
        {
            _moduleOutput = new  OnlineGameModuleOutput(_playManager);
            _moduleInput = new  OnlineGameModuleInput(_playManager);
        }

        public override void Dispose()
        {
            _moduleInput.Dispose();
            _moduleOutput.Dispose();
        }

        public override void OnEnable()
        {
            _moduleInput.OnEnable();
            _moduleOutput.OnEnable();
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
        }

        public override void OnDisable()
        {
            _moduleInput.OnDisable();
            _moduleOutput.OnDisable();
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
        }

        private void onClickBack(object obj)
        {
            OnDisable();
            _playManager.Module<LobbyModule>().OnEnable();
        }

    }
}
