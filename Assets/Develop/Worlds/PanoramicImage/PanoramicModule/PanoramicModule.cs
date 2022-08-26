using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.PanoramicImage
{
    public class  PanoramicModule : Part<PanoramicImagePlayManager>
    {
        private  PanoramicModuleInput _moduleInput;
        private  PanoramicModuleOutput _moduleOutput;

        public PanoramicModule(WorldBase playManager) : base(playManager)
        {
            Cursor.lockState = CursorLockMode.Locked;
            _moduleInput = new  PanoramicModuleInput(_world);
            _moduleOutput = new  PanoramicModuleOutput(_world);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
        }

        public override void Dispose()
        {
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);

            _moduleInput.Dispose();
            _moduleOutput.Dispose();
            base.Dispose();
        }

        private void onClickBack(object data)
        {
            _world.Destroy();
            new GameLobby.GameLobbyPlayManager().Create();
        }

    }
}