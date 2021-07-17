using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace GamePlay.GameLobby
{
    public class OnlineGameModuleOutput : IModuleOutput
    {
        GameLobbyPlayManager _playManager;
        private GameItemDatas _gameItemDatas;
        private OnlineGameUIComps _uiComps;
        private Dictionary<float,PB_OnlineGame> _onlineGameDic = new Dictionary<float, PB_OnlineGame>();

        public OnlineGameModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickCreateGameBtn,onClickCreateGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickOnlineGameBtn,onClickOnlineGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickStartBtn,onClickStartBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickExitBtn,onClickExitBtn);

            _uiComps = GameObject.Find("UINode/onlineGameUI").GetComponent<OnlineGameUIComps>();
            showGameDataList();
            _uiComps.Nickname.text = ConfigDatabase.GetConfig("nickname",SystemInfo.deviceName);
            _uiComps.Nickname.onEndEdit.AddListener(onNicknameEndEdit);
        }

        public void Dispose()
        {
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
            waitEnterOnlineGameUI(aniTime).Start();
            
            Cursor.lockState = CursorLockMode.None;
        }

        IEnumerator waitEnterOnlineGameUI(float time)
        {
            yield return new WaitForSeconds(time);
        }

        private void showGameDataList()
        {
            _uiComps.Online.gameObject.SetActive(true);
            _uiComps.Create.gameObject.SetActive(true);
            _uiComps.Start.gameObject.SetActive(false);
            _uiComps.Exit.gameObject.SetActive(false);
            int idx=-1;
            foreach(var gameData in _playManager.GameDatas)
            {
                idx++;
                Transform item_t = null;
                if(idx<_uiComps.ItemList.childCount)
                {
                    item_t = _uiComps.ItemList.GetChild(idx);
                }
                else
                {
                    item_t = GameObject.Instantiate(_uiComps.ItemList.GetChild(0).gameObject,_uiComps.ItemList).transform;
                }
                resetGameDataItem(gameData,item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = _playManager.GameDatas.Count; i < _uiComps.ItemList.childCount; i++)
            {
                _uiComps.ItemList.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void resetGameDataItem(GameItemData data,Transform item_t)
        {
            item_t.GetChild(0).GetComponent<Image>().sprite = data.Icon;
            item_t.GetChild(1).GetComponent<Text>().text = data.Name;
            item_t.GetComponent<Button>().onClick.RemoveAllListeners();
            item_t.GetComponent<Button>().onClick.AddListener(()=>
            {
                showOnlineGameInfo(data);
            });
        }

        private void showOnlineGameList()
        {
            _uiComps.Online.gameObject.SetActive(true);
            _uiComps.Create.gameObject.SetActive(true);
            _uiComps.Start.gameObject.SetActive(false);
            _uiComps.Exit.gameObject.SetActive(false);
            var datas =_onlineGameDic.GetEnumerator();
            for (int i = 0; i < _onlineGameDic.Count; i++)
            {
                Transform item_t = null;
                if(i<_uiComps.ItemList.childCount)
                {
                    item_t = _uiComps.ItemList.GetChild(i);
                }
                else
                {
                    item_t = GameObject.Instantiate(_uiComps.ItemList.GetChild(0).gameObject,_uiComps.ItemList).transform;
                }
                datas.MoveNext();
                resetOnlineGameItem(datas.Current.Value,item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = _onlineGameDic.Count; i < _uiComps.ItemList.childCount; i++)
            {
                _uiComps.ItemList.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void resetOnlineGameItem(PB_OnlineGame data,Transform item_t)
        {
            item_t.GetChild(0).GetComponent<Image>().sprite = null;
            item_t.GetChild(1).GetComponent<Text>().text = $"{data.GamePlayID} {_playManager.GameDatas[data.GameID].Name}";
            item_t.GetComponent<Button>().onClick.RemoveAllListeners();
            item_t.GetComponent<Button>().onClick.AddListener(()=>
            {
                showOnlineGameInfo(null);
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

        private void showOnlineGameInfo(GameItemData data)
        {
            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickExitBtn,1);
            _uiComps.Online.gameObject.SetActive(false);
            _uiComps.Create.gameObject.SetActive(false);
            _uiComps.Start.gameObject.SetActive(true);
            _uiComps.Exit.gameObject.SetActive(true);
            var datas =new List<object>();
            for (int i = 0; i < datas.Count; i++)
            {
                Transform item_t = null;
                if(i<_uiComps.ItemList.childCount)
                {
                    item_t = _uiComps.ItemList.GetChild(i);
                }
                else
                {
                    item_t = GameObject.Instantiate(_uiComps.ItemList.GetChild(0).gameObject,_uiComps.ItemList).transform;
                }
                resetOnlineGameInfoItem(datas[i],item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = datas.Count; i < _uiComps.ItemList.childCount; i++)
            {
                _uiComps.ItemList.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void resetOnlineGameInfoItem(object data,Transform item_t)
        {
            // item_t.GetChild(0).GetComponent<Image>().sprite = data.Icon;
            // item_t.GetChild(1).GetComponent<Text>().text = data.Name;
        }
    }
}
