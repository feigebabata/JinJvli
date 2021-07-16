using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.GameLobby
{
    public class  OnlineGameModule : PlayModule<GameLobbyPlayManager>
    {
        private  OnlineGameModuleInput _moduleInput;
        private  OnlineGameModuleOutput _moduleOutput;
        public override void OnInit(PlayManager playManager)
        {
            if(IsInit)
            {
                return;
            }
            base.OnInit(playManager);
            //code
            _moduleOutput = new  OnlineGameModuleOutput(_playManager);
            _moduleInput = new  OnlineGameModuleInput(_playManager);
        }

        public override void OnRelease()
        {
            if(!IsInit)
            {
                return;
            }
            //code

            _moduleInput.Dispose();
            _moduleOutput.Dispose();
            base.OnRelease();
        }

        public override void OnShow()
        {
            base.OnShow();
            //code
        }

        public override void OnHide()
        {
            base.OnHide();
            //code
        }

    }
}
