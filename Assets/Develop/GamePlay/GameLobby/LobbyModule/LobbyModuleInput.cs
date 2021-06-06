using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using UnityEngine.EventSystems;

namespace GamePlay.GameLobby
{
    public class LobbyModuleInput : IDisposable
    {
        GameLobbyPlayManager _playManager;
        private Camera _mainCamera;
        private GameItem _currectSelect;

        public LobbyModuleInput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;

            _mainCamera = GameObject.Find("character/Main Camera").GetComponent<Camera>();

            MonoBehaviourEvent.I.UpdateListener += Update;
        }
        
        public void Dispose()
        {
            MonoBehaviourEvent.I.UpdateListener -= Update;
            _playManager = null;
            _mainCamera=null;
        }

        private void Update()
        {
            rayUpdate();

            if(Input.GetMouseButtonDown(0) && EventSystem.current && !EventSystem.current.firstSelectedGameObject)
            {
                if(_currectSelect!=null)
                {
                    _playManager.Messenger.Broadcast(GameLobbyMsgID.OnEnterGame,_currectSelect.TypeName);
                }
            }
            Vector2 dir = Vector2.zero;
            if(Input.GetKey(KeyCode.W))
            {
                dir+=Vector2.up;
            }
            if(Input.GetKey(KeyCode.S))
            {
                dir+=Vector2.down;
            }
            if(Input.GetKey(KeyCode.A))
            {
                dir+=Vector2.left;
            }
            if(Input.GetKey(KeyCode.D))
            {
                dir+=Vector2.right;
            }
            if(dir!=Vector2.zero)
            {
                _playManager.Messenger.Broadcast(GameLobbyMsgID.OnMove,dir.normalized);
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

    }
}
