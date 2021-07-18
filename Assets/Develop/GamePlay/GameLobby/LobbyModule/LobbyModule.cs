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

        public LobbyModule(PlayManager playManager) : base(playManager)
        {

            _moduleInput = new  LobbyModuleInput(_playManager);
            _moduleOutput = new  LobbyModuleOutput(_playManager);
            //code


            GameObject.Find("cameraAnim").GetComponent<PlayableDirector>().stopped += onStartAniStop;

            _moduleOutput.ShowItemList().Start();
        }

        public override void Dispose()
        {
            _moduleInput.Dispose();
            _moduleOutput.Dispose();

            Cursor.lockState = CursorLockMode.None;
        }

        public override void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            _moduleInput.OnEnable();
            _moduleOutput.OnEnable();
            addListener();
        }

        public override void OnDisable()
        {
            _moduleInput.OnDisable();
            _moduleOutput.OnDisable();
            removeListener();
        }

        private void addListener()
        {
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterSelectGame,onEnterSelectGame);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
            RaycastEvent.I.AddListener(RaycastEventType.Click,onRaycastClick);
        }

        private void removeListener()
        {
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterSelectGame,onEnterSelectGame);
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            RaycastEvent.I.RemoveListener(RaycastEventType.Click,onRaycastClick);
        }

        private void onClickBack(object obj)
        {
            Application.Quit();
        }

        private void onEnterSelectGame(object obj)
        {

            var typeName = obj as string;
            
            OnDisable();
            _playManager.Module<OnlineGameModule>().OnEnable();
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnEnterOnlineGame,_playManager.GameDatas[typeName]);
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
                OnDisable();
                _playManager.Module<OnlineGameModule>().OnEnable();
                _playManager.Messenger.Broadcast(GameLobbyMsgID.OnEnterOnlineGame,null);
            }
        }

        

    }
}
