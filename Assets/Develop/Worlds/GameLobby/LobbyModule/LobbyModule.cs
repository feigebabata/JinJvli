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
    public class  LobbyModule : Part<GameLobbyPlayManager>
    {
        private  LobbyModuleInput _moduleInput;
        private  LobbyModuleOutput _moduleOutput;

        public LobbyModule(WorldBase playManager) : base(playManager)
        {

            _moduleInput = new  LobbyModuleInput(_world);
            _moduleOutput = new  LobbyModuleOutput(_world);
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
            _world.Messenger.Add(GameLobbyMsgID.OnEnterSelectGame,onEnterSelectGame);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
            RaycastEventSystem.I.AddListener(RaycastEventType.Click,onRaycastClick);
        }

        private void removeListener()
        {
            _world.Messenger.Remove(GameLobbyMsgID.OnEnterSelectGame,onEnterSelectGame);
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            RaycastEventSystem.I.RemoveListener(RaycastEventType.Click,onRaycastClick);
        }

        private void onClickBack(object obj)
        {
            Application.Quit();
        }

        private void onEnterSelectGame(object obj)
        {

            var typeName = obj as string;
            
            OnDisable();
            _world.Part<OnlineGameModule>().OnEnable();
            _world.Messenger.Broadcast(GameLobbyMsgID.OnEnterOnlineGame,_world.GameDatas[typeName]);
        }

        private void onStartAniStop(PlayableDirector obj)
        {
            _world.Messenger.Broadcast(GameLobbyMsgID.OnStartAniStop,null);
        }

        private void onRaycastClick(RaycastHit obj)
        {
            // Debug.Log(obj.transform.name);
            if(obj.transform.name=="UINode")
            {
                OnDisable();
                _world.Part<OnlineGameModule>().OnEnable();
                _world.Messenger.Broadcast(GameLobbyMsgID.OnEnterOnlineGame,null);
            }
        }

        

    }
}