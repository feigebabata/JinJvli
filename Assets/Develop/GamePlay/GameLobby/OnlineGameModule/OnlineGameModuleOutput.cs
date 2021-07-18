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
        private UIListType _currentList = UIListType.CreateGame;
        private long _selectGamePlayID;
        private Coroutine _resetListItem;

        public OnlineGameModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickCreateGameBtn,onClickCreateGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickOnlineGameBtn,onClickOnlineGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickStartBtn,onClickStartBtn);
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
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickStartBtn,onClickStartBtn);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickExitBtn,onClickExitBtn);
            _playManager = null;
        }

        public void OnEnable()
        {
        }

        public void OnDisable()
        {
        }

        private void onEnterOnlineGame(object obj)
        {
            var endPos = new Vector3(-3.8f,1,-6);
            var endAngle = new Vector3(-10,-90,0);
            float aniTime = 2;
            var character = GameObject.Find("character").transform;
            character.MoveLocal(endPos,aniTime).Start();
            Camera.main.transform.RotateLocal(endAngle,aniTime).Start();
            
            Cursor.lockState = CursorLockMode.None;
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
            PB_OnlineGame gameplay = new PB_OnlineGame();
            gameplay.GameID = data.ID;
            gameplay.Ready = false;
            gameplay.Player = new PB_PlayerInfo()
            {
                Nickname = ConfigDatabase.GetConfig("nickname",SystemInfo.deviceName),
                ID = SystemInfo.deviceUniqueIdentifier,
            };
            gameplay.GamePlayID = DateTime.Now.UnixMilliseconds();

            _selectGamePlayID = gameplay.GamePlayID;
            showOnlineGameInfo();
            _playManager.Messenger.Broadcast(GameLobbyMsgID.OnCreateGame,gameplay);
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
            var dataDic = new Dictionary<long,List<PB_OnlineGame>>();

            lock(module.OnlineGameDicLock)
            {
                List<float> removes = new List<float>();
                foreach (var item in module.OnlineGameDic)
                {
                    //离线3秒 认为退出
                    // Debug.Log($"{Time.time}  {item.Key}");
                    if(Time.time-item.Key>3)
                    {
                        removes.Add(item.Key);
                    }
                }
                foreach (var key in removes)
                {
                    module.OnlineGameDic.Remove(key);
                }

                foreach (var item in module.OnlineGameDic)
                {
                    if(dataDic.ContainsKey(item.Value.GamePlayID))
                    {
                        dataDic[item.Value.GamePlayID].Add(item.Value);
                    }
                    else
                    {
                        dataDic.Add(item.Value.GamePlayID,new List<PB_OnlineGame>(){item.Value});
                    }
                    dataDic[item.Value.GamePlayID].Sort((l,r)=>{return (int)(l.GamePlayID-r.GamePlayID);});
                }

            }
            var dictSort = from kv in dataDic orderby kv.Key  descending select kv;
            _uiComps.ItemList.ResetListItem<KeyValuePair<long, List<PB_OnlineGame>>>(dictSort.GetEnumerator(),resetOnlineGameItem);

        }

        private void resetOnlineGameItem(KeyValuePair<long, List<PB_OnlineGame>> list,Transform item_t)
        {
            _uiComps.NoList.SetActive(false);
            var gamedata = _playManager.GameDatas[list.Value[0].GameID];
            string content = gamedata.Name+" : ";
            foreach (var onlineGame in list.Value)
            {
                content = content + onlineGame.Player.Nickname+" ";
            }
            content = content + Color.yellow.RichText($"({list.Value.Count}/{gamedata.PlayerMaxCount})");

            item_t.GetChild(0).GetComponent<Image>().sprite = null;
            item_t.GetChild(1).GetComponent<Text>().text = content;
            item_t.GetComponent<Button>().onClick.RemoveAllListeners();
            item_t.GetComponent<Button>().onClick.AddListener(()=>
            {
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
        }

        private void onClickStartBtn(object obj)
        {
            
        }

        private void onNicknameEndEdit(string arg0)
        {
            if(string.IsNullOrEmpty(arg0) || string.IsNullOrWhiteSpace(arg0))
            {
                _uiComps.Nickname.text = SystemInfo.deviceName;
            }
            ConfigDatabase.SetConfig("nickname",_uiComps.Nickname.text);
        }

        private void showOnlineGameInfo()
        {
            _uiComps.NoList.SetActive(false);
            _currentList = UIListType.GamePlay;
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickExitBtn,1);
            _uiComps.Online.gameObject.SetActive(false);
            _uiComps.Create.gameObject.SetActive(false);
            _uiComps.Start.gameObject.SetActive(true);
            _uiComps.Exit.gameObject.SetActive(true);
            var datas =new List<PB_OnlineGame>();
            foreach (var item in _playManager.Module<OnlineGameModule>().OnlineGameDic)
            {
                if(item.Value.GamePlayID==_selectGamePlayID)
                {
                    datas.Add(item.Value);
                }
            }
            _uiComps.ItemList.ResetListItem<PB_OnlineGame>(datas.GetEnumerator(),resetOnlineGameInfoItem);
        }

        private void resetOnlineGameInfoItem(PB_OnlineGame data,Transform item_t)
        {
            item_t.GetChild(0).GetComponent<Image>().sprite = null;
            item_t.GetChild(1).GetComponent<Text>().text = data.Player.Nickname;
        }

        IEnumerator resetListItem()
        {
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
                yield return new WaitForSeconds(1);
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
