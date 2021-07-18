using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using UnityEngine.EventSystems;
using FGUFW.Play;

namespace GamePlay.GameLobby
{
    public class LobbyModuleInput : IModuleInput
    {
        GameLobbyPlayManager _playManager;
        private Camera _mainCamera;
        private GameItem _currectSelect;

        public LobbyModuleInput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;

            _mainCamera = GameObject.Find("character/Main Camera").GetComponent<Camera>();

        }

        public void Dispose()
        {
            _playManager = null;
            _mainCamera=null;
        }

        public void OnEnable()
        {
            MonoBehaviourEvent.I.UpdateListener += Update;
            _playManager.Messenger.Add(GameLobbyMsgID.OnStartAniStop,onStartAniStop);
            SetCharacterCtrl(true);
        }

        public void OnDisable()
        {
            MonoBehaviourEvent.I.UpdateListener -= Update;
            _playManager.Messenger.Remove(GameLobbyMsgID.OnStartAniStop,onStartAniStop);
            SetCharacterCtrl(false);
        }

        private void Update()
        {
            rayUpdate();

            if(Input.GetMouseButtonDown(0) && EventSystem.current && !EventSystem.current.firstSelectedGameObject)
            {
                if(_currectSelect!=null)
                {
                    _playManager.Messenger.Broadcast(GameLobbyMsgID.OnEnterSelectGame,_currectSelect.TypeName);
                }
            }

        }

        void rayUpdate()
        {
            Ray ray = new Ray(_mainCamera.transform.position,_mainCamera.transform.forward);
            RaycastHit raycastHit;
            if(Physics.Raycast(ray,out raycastHit))
            {
                var item = raycastHit.transform.GetComponent<GameItem>();

                if(item)
                {
                    
                    if(_currectSelect && _currectSelect!=item)
                    {
                        _playManager.Messenger.Broadcast(GameLobbyMsgID.OnUnSelectGameItem,_currectSelect);
                    }

                    if(_currectSelect==null || _currectSelect!=item)
                    {
                        _currectSelect = item;
                        _playManager.Messenger.Broadcast(GameLobbyMsgID.OnSelectGameItem,_currectSelect);
                    }
                }
                else if(_currectSelect)
                {
                    _playManager.Messenger.Broadcast(GameLobbyMsgID.OnUnSelectGameItem,_currectSelect);
                    _currectSelect=null;
                }
                
            }
            
        }

        private void onStartAniStop(object obj)
        {
            
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    _mainCamera.gameObject.AddComponent<GyroRotateCtrl>();
                    _mainCamera.gameObject.AddComponent<TouchMoveCtrl>().OnMove += onMove;
                }
                break;
                default:
                {
                    _mainCamera.gameObject.AddComponent<MouseRotateCtrl>();
                    _mainCamera.gameObject.AddComponent<KeyboardMoveCtrl>().OnMove += onMove;
                }
                break;
            }
        }

        private void onMove(Vector2 obj)
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnMove,obj);
        }

        public void SetCharacterCtrl(bool enable)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                {
                    if(_mainCamera.gameObject.GetComponent<GyroRotateCtrl>())
                    {
                        _mainCamera.gameObject.GetComponent<GyroRotateCtrl>().enabled = enable;
                    }
                    if(_mainCamera.gameObject.GetComponent<TouchMoveCtrl>())
                    {
                        _mainCamera.gameObject.GetComponent<TouchMoveCtrl>().enabled = enable;
                    }
                }
                break;
                default:
                {
                    if(_mainCamera.gameObject.GetComponent<MouseRotateCtrl>())
                    {
                        _mainCamera.gameObject.GetComponent<MouseRotateCtrl>().enabled = enable;
                    }
                    if(_mainCamera.gameObject.GetComponent<KeyboardMoveCtrl>())
                    {
                        _mainCamera.gameObject.GetComponent<KeyboardMoveCtrl>().enabled = enable;
                    }
                }
                break;
            }
        }
    }
}
