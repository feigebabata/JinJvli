using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.UI;

namespace GamePlay.GameLobby
{
    public class OnlineGameModuleInput : IPartInput
    {
        private GameLobbyPlayManager _playManager;
        private OnlineGameUIComps _uiComps;

        public OnlineGameModuleInput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            
            _uiComps = GameObject.Find("UINode/onlineGameUI").GetComponent<OnlineGameUIComps>();
        }

        public void Dispose()
        {
            _playManager = null;
        }

        public void OnEnable()
        {
            addUIListener();
        }

        public void OnDisable()
        {
            removeUIListener();
        } 

        void addUIListener()
        {
            _uiComps.Create.onClick.AddListener(onClickCreateBtn);
            _uiComps.Online.onClick.AddListener(onClickOnlineBtn);
            _uiComps.Start.onClick.AddListener(onClickStartBtn);
            _uiComps.Exit.onClick.AddListener(onClickExitBtn);
        }

        void removeUIListener()
        {
            _uiComps.Create.onClick.RemoveAllListeners();
            _uiComps.Online.onClick.RemoveAllListeners();
            _uiComps.Start.onClick.RemoveAllListeners();
            _uiComps.Exit.onClick.RemoveAllListeners();
        }

        private void onClickOnlineBtn()
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnClickOnlineGameBtn,null);
        }

        private void onClickCreateBtn()
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnClickCreateGameBtn,null);
        }

        private void onClickStartBtn()
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnClickStartBtn,null);
        }

        private void onClickExitBtn()
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnClickExitBtn,null);
        }
    }
}
