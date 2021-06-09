using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.PanoramicImage
{
    public class  PanoramicModule : PlayModule<PanoramicImagePlayManager>
    {
        private  PanoramicModuleInput _moduleInput;
        private  PanoramicModuleOutput _moduleOutput;
        public override void OnInit(PlayManager playManager)
        {
            Cursor.lockState = CursorLockMode.Locked;
            if(IsInit)
            {
                return;
            }
            base.OnInit(playManager);
            //code
            _moduleInput = new  PanoramicModuleInput(_playManager);
            _moduleOutput = new  PanoramicModuleOutput(_playManager);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);

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

        public override void OnRelease()
        {
            Cursor.lockState = CursorLockMode.None;

            if(!IsInit)
            {
                return;
            }
            //code
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);

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

        private void onClickBack(object data)
        {
            _playManager.Destroy();
            new GameLobby.GameLobbyPlayManager().Create();
        }

    }
}