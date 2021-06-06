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
        private Camera _mainCamera;
        private CharacterController _characterController;
        private float Speed=3;

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
            _playManager.Messenger.Add(GameLobbyMsgID.OnMove,onMove);
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterGame,onEnterGame);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);

            _mainCamera = GameObject.Find("character/Main Camera").GetComponent<Camera>();
            _characterController = GameObject.Find("character").GetComponent<CharacterController>();

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
            _playManager.Messenger.Remove(GameLobbyMsgID.OnMove,onMove);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterGame,onEnterGame);
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);

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
        }

        private void onEnterGame(object obj)
        {
            var typeName = obj as string;
            Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
            var playManager = assembly.CreateInstance(typeName) as PlayManager; 

            _playManager.Destroy();
            playManager.Create();
        }

        private void onMove(object data)
        {
            var v2 = (Vector2)data;
            var selfDirUp = new Vector3(_mainCamera.transform.forward.x,0,_mainCamera.transform.forward.z).normalized*v2.y*Speed; 
            var selfDirRight = new Vector3(_mainCamera.transform.right.x,0,_mainCamera.transform.right.z).normalized*v2.x*Speed;
            _characterController.SimpleMove(selfDirRight+selfDirUp);
        }

        private void onStartAniStop(PlayableDirector obj)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            _mainCamera.GetComponent<AndroidCameraRotateCtrl>().enabled=true;
            _mainCamera.GetComponent<AndroidCameraRotateCtrl>().PlayerInput = _playManager.PlayerInput as AndroidPlayerInput;
            #else
            _mainCamera.GetComponent<PCCameraRotateCtrl>().enabled=true;
            #endif
        }

    }
}
