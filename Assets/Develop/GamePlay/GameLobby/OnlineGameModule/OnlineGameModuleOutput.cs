using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Google.Protobuf;
using System.Linq;

namespace GamePlay.GameLobby
{
    public class OnlineGameModuleOutput : IModuleOutput
    {
        GameLobbyPlayManager _playManager;
        private GameItemDatas _gameItemDatas;
        private OnlineGameUIComps _uiComps;
        private UIListType _currentList = UIListType.OnlineGame;
        private Coroutine _resetListItem;
        private Coroutine _characterMoveCor;
        private Coroutine _characterRotateCor;

        public OnlineGameModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickCreateGameBtn,onClickCreateGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickOnlineGameBtn,onClickOnlineGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickExitBtn,onClickExitBtn);

            _uiComps = GameObject.Find("UINode/onlineGameUI").GetComponent<OnlineGameUIComps>();
            _uiComps.Nickname.text = ConfigDatabase.GetConfig("nickname",SystemInfo.deviceName);
            _uiComps.Nickname.onEndEdit.AddListener(onNicknameEndEdit);

            _resetListItem = resetListItem().Start();
        }

        public void Dispose()
        {
            _resetListItem?.Stop();
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickCreateGameBtn,onClickCreateGameBtn);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickOnlineGameBtn,onClickOnlineGameBtn);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickExitBtn,onClickExitBtn);
            _playManager = null;
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickExitBtn);
            _characterMoveCor?.Stop();
            _characterRotateCor?.Stop();
        }

        private void onEnterOnlineGame(object obj)
        {
            var endPos = new Vector3(-3.8f,1,-6);
            var endAngle = new Vector3(-10,-90,0);
            float aniTime = 2;
            var character = GameObject.Find("character").transform;
            _characterMoveCor = character.MoveLocal(endPos,aniTime).Start();
            _characterRotateCor = Camera.main.transform.RotateLocal(endAngle,aniTime).Start();
            
            Cursor.lockState = CursorLockMode.None;

            if(obj!=null)
            {
                var gamedata = obj as GameItemData;
                createGamePlay(gamedata);
            }
        }

        private void showGameDataList()
        {
            _uiComps.NoList.SetActive(false);
            _currentList = UIListType.CreateGame;
            _uiComps.Online.gameObject.SetActive(true);
            _uiComps.Create.gameObject.SetActive(true);
            _uiComps.Start.gameObject.SetActive(false);
            _uiComps.Exit.gameObject.SetActive(false);
            var datas = _playManager.GameDatas.GetEnumerator();
            _uiComps.ItemList.ResetListItem<GameItemData>(datas,resetGameDataItem);
        }

        private void resetGameDataItem(GameItemData data,Transform item_t)
        {
            item_t.GetChild(0).GetComponent<Image>().sprite = data.Icon;
            item_t.GetChild(1).GetComponent<Text>().text = data.Name;
            item_t.GetComponent<Button>().onClick.RemoveAllListeners();
            item_t.GetComponent<Button>().onClick.AddListener(()=>
            {
                createGamePlay(data);
            });
        }

        private void createGamePlay(GameItemData data)
        {
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnCreateGame,data);
            showOnlineGameInfo();
        }

        private void showOnlineGameList()
        {
            _uiComps.NoList.SetActive(true);
            _currentList = UIListType.OnlineGame;
            _uiComps.Online.gameObject.SetActive(true);
            _uiComps.Create.gameObject.SetActive(true);
            _uiComps.Start.gameObject.SetActive(false);
            _uiComps.Exit.gameObject.SetActive(false);
            var module = _playManager.Module<OnlineGameModule>();

            var dictSort = from kv in module.OnlineGameDic orderby kv.Key  descending select kv;
            _uiComps.ItemList.ResetListItem<KeyValuePair<long, PB_OnlineGame>>(dictSort.GetEnumerator(),resetOnlineGameItem);

        }

        private void resetOnlineGameItem(KeyValuePair<long, PB_OnlineGame> kv,Transform item_t)
        {
            _uiComps.NoList.SetActive(false);
            var onlineGame = kv.Value;
            var gamedata = _playManager.GameDatas[onlineGame.GameID];
            string content = gamedata.Name+" : ";
            foreach (var player in onlineGame.Players)
            {
                content = content + player.PlayerInfo.Nickname+" ";
            }
            content = content + Color.yellow.RichText($"({onlineGame.Players.Count}/{gamedata.PlayerMaxCount})");

            item_t.GetChild(0).GetComponent<Image>().sprite = gamedata.Icon;
            item_t.GetChild(1).GetComponent<Text>().text = content;
            item_t.GetComponent<Button>().onClick.RemoveAllListeners();
            item_t.GetComponent<Button>().onClick.AddListener(()=>
            {
                _playManager.Messenger.Broadcast(GameLobbyMsgID.OnJoinGame,onlineGame);

                _playManager.Module<OnlineGameModule>().SelectGamePlayID = onlineGame.GamePlayID;
                showOnlineGameInfo();
            });
        }

        private void onClickOnlineGameBtn(object obj)
        {
            showOnlineGameList();
        }

        private void onClickCreateGameBtn(object obj)
        {
            showGameDataList();
        }

        private void onClickExitBtn(object obj)
        {
            GlobalMessenger.M.Abort(GlobalMsgID.OnBackKey);
            showOnlineGameList();
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickExitBtn);
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnExitGame,null);
        }

        private void onNicknameEndEdit(string arg0)
        {
            if(string.IsNullOrEmpty(arg0) || string.IsNullOrWhiteSpace(arg0))
            {
                _uiComps.Nickname.text = _playManager.Module<OnlineGameModule>().SelfInfo.Nickname;
            }
            _playManager.Module<OnlineGameModule>().SelfInfo.Nickname = _uiComps.Nickname.text;
            ConfigDatabase.SetConfig("nickname",_uiComps.Nickname.text);
        }

        private void showOnlineGameInfo()
        {
            _currentList = UIListType.GamePlay;
            _uiComps.NoList.SetActive(true);
            if(!_playManager.Module<OnlineGameModule>().OnlineGameDic.ContainsKey(_playManager.Module<OnlineGameModule>().SelectGamePlayID))
            {
                return;
            }
            _uiComps.NoList.SetActive(false);
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickExitBtn,1);
            _uiComps.Online.gameObject.SetActive(false);
            _uiComps.Create.gameObject.SetActive(false);
            _uiComps.Start.gameObject.SetActive(true);
            _uiComps.Exit.gameObject.SetActive(true);
            
            var onlineGame = _playManager.Module<OnlineGameModule>().OnlineGameDic[_playManager.Module<OnlineGameModule>().SelectGamePlayID];
            _uiComps.ItemList.ResetListItem<PB_Player>(onlineGame.Players.GetEnumerator(),resetOnlineGameInfoItem);
        }

        private void resetOnlineGameInfoItem(PB_Player player,Transform item_t)
        {
            item_t.GetChild(0).GetComponent<Image>().sprite = null;
            item_t.GetChild(1).GetComponent<Text>().text = player.PlayerInfo.Nickname;
        }

        IEnumerator resetListItem()
        {
            yield return null;
            while (true)
            {
                switch (_currentList)
                {
                    case UIListType.CreateGame:
                        showGameDataList();
                    break;
                    case UIListType.OnlineGame:
                        showOnlineGameList();
                    break;
                    case UIListType.GamePlay:
                        showOnlineGameInfo();
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }
        }

        public enum UIListType
        {
            CreateGame,
            OnlineGame,
            GamePlay,
        }
    }
}
