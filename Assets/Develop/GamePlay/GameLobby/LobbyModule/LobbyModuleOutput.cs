using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.GameLobby
{
    public class LobbyModuleOutput : IModuleOutput
    {
        GameLobbyPlayManager _playManager;
        private Transform _gameItemsParent;
        private Color _select = new Color32(255,0,255,255);
        private Color _unSelect = new Color32(0,255,255,255);
        private Camera _mainCamera;
        private CharacterController _characterController;
        private float _speed=3;

        public LobbyModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _gameItemsParent = GameObject.Find("gamelist").transform;
            _mainCamera = GameObject.Find("character/Main Camera").GetComponent<Camera>();
            _characterController = GameObject.Find("character").GetComponent<CharacterController>();
            
            _playManager.Messenger.Add(GameLobbyMsgID.OnUnSelectGameItem,onUnSelectGameItem);
            _playManager.Messenger.Add(GameLobbyMsgID.OnSelectGameItem,onSelectGameItem);
            _playManager.Messenger.Add(GameLobbyMsgID.OnMove,onMove);
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(GameLobbyMsgID.OnUnSelectGameItem,onUnSelectGameItem);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnSelectGameItem,onSelectGameItem);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnMove,onMove);

            _playManager = null;
            _mainCamera = null;
            _characterController = null;
        }

        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
        }

        public IEnumerator ShowItemList()
        {
            bool ignore = false;
            int i = -1;
            foreach (var gameData in _playManager.GameDatas)
            {
                i++;
                ignore = !Application.isEditor && Array.IndexOf<RuntimePlatform>(gameData.Platform,Application.platform)!=-1;
                if(!ignore)
                {
                    Transform item = GameObject.Instantiate(_gameItemsParent.GetChild(0),_gameItemsParent,false);
                    item.GetComponent<GameItem>().TypeName = gameData.TypeName;
                    item.GetChild(0).GetComponent<SpriteRenderer>().sprite = gameData.Icon;
                    item.GetChild(0).localScale = gameData.Scale;
                    var space = item.localPosition*(1+i*0.02f);
                    item.localPosition = Quaternion.Euler(0,23*i,0) * space;
                    item.gameObject.SetActive(true);
                    item.name = gameData.TypeName;
                }
                yield return new WaitForSeconds(1);
            }
        }

        private void onUnSelectGameItem(object obj)
        {
            var item = obj as GameItem;
            item.GetComponent<MeshRenderer>().material.SetColor("MainColor",_unSelect);
        }
        
        private void onSelectGameItem(object obj)
        {
            var item = obj as GameItem;
            item.GetComponent<MeshRenderer>().material.SetColor("MainColor",_select);
        }

        private void onMove(object data)
        {
            var v2 = (Vector2)data;
            var selfDirUp = new Vector3(_mainCamera.transform.forward.x,0,_mainCamera.transform.forward.z).normalized*v2.y*_speed; 
            var selfDirRight = new Vector3(_mainCamera.transform.right.x,0,_mainCamera.transform.right.z).normalized*v2.x*_speed;
            _characterController.SimpleMove(selfDirRight+selfDirUp);
        }

        private void onClickOnlineGame()
        {

        }

        void enterOnlineGame()
        {
            var endPos = new Vector3(-4,1,-6);
            var endAngle = new Vector3(-10,0,-90);
            float aniTime = 2;
            var character = GameObject.Find("character").transform;
            character.MoveLocal(endPos,aniTime);
            Camera.main.transform.RotateLocal(endAngle,aniTime);
        }

    }
}
