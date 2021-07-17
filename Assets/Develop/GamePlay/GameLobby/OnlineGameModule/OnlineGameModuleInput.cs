using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.UI;

namespace GamePlay.GameLobby
{
    public class OnlineGameModuleInput : IDisposable
    {
        private GameLobbyPlayManager _playManager;
        private Transform _itemListNode;
        private Button _createBtn,_onlineBtn; 

        public OnlineGameModuleInput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);

            _itemListNode = GameObject.Find("UINode/onlineGameUI/itemlist/Viewport/Content").transform;
            _createBtn = GameObject.Find("UINode/onlineGameUI/switch/online").GetComponent<Button>();
            _onlineBtn = GameObject.Find("UINode/onlineGameUI/switch/create").GetComponent<Button>();
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager = null;
        }

        void addUIListener()
        {
            _createBtn.onClick.AddListener(onClickCreateBtn);
            _onlineBtn.onClick.AddListener(onClickOnlineBtn);
        }

        void removeUIListener()
        {
            _createBtn.onClick.RemoveAllListeners();
            _onlineBtn.onClick.RemoveAllListeners();
        }

        private void onClickOnlineBtn()
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnClickCreateGameBtn,null);
        }

        private void onClickCreateBtn()
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnClickOnlineGameBtn,null);
        }

        private void onEnterOnlineGame(object obj)
        {
            addUIListener();
        }

    }
}
