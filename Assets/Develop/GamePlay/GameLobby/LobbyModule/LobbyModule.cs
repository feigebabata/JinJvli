using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;
using UnityEngine.Playables;
using System.Reflection;

namespace GamePlay.GameLobby
{
    public class  LobbyModule : PlayModule<GameLobbyPlayManager>
    {
        private  LobbyModuleInput _moduleInput;
        private  LobbyModuleOutput _moduleOutput;

        public override void OnInit(PlayManager playManager)
        {
            if(IsInit)
            {
                return;
            }
            base.OnInit(playManager);
            
            Cursor.lockState = CursorLockMode.Locked;

            _moduleInput = new  LobbyModuleInput(_playManager);
            _moduleOutput = new  LobbyModuleOutput(_playManager);
            //code
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterGame,onEnterGame);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
            RaycastEvent.I.AddListener(RaycastEventType.Click,onRaycastClick);


            GameObject.Find("cameraAnim").GetComponent<PlayableDirector>().stopped += onStartAniStop;

            _moduleOutput.ShowItemList().Start();
        }

        public override void OnRelease()
        {
            if(!IsInit)
            {
                return;
            }
            //code
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterGame,onEnterGame);
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            RaycastEvent.I.RemoveListener(RaycastEventType.Click,onRaycastClick);

            _moduleInput.Dispose();
            _moduleOutput.Dispose();

            Cursor.lockState = CursorLockMode.None;
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

        private void onClickBack(object obj)
        {
            Application.Quit();
        }

        private void onEnterGame(object obj)
        {
            var typeName = obj as string;
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            var playManager = assembly.CreateInstance(typeName) as PlayManager; 

            _playManager.Destroy();
            playManager.Create();
        }

        private void onStartAniStop(PlayableDirector obj)
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnStartAniStop,null);
        }

        private void onRaycastClick(RaycastHit obj)
        {
            // Debug.Log(obj.transform.name);
            if(obj.transform.name=="UINode")
            {
                _moduleInput.SetCharacterCtrl(false);
                _playManager.Messenger.Broadcast(GameLobbyMsgID.OnEnterOnlineGame,obj.transform.Find("onlineGameUI"));
            }
        }

    }
}
