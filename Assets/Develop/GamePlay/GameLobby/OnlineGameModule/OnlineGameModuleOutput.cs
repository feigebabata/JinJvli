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
    public class OnlineGameModuleOutput : IDisposable
    {
        GameLobbyPlayManager _playManager;
        private GameItemDatas _gameItemDatas;
        private Transform _itemListNode;

        public OnlineGameModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickCreateGameBtn,onClickCreateGameBtn);
            _playManager.Messenger.Add(GameLobbyMsgID.OnClickOnlineGameBtn,onClickOnlineGameBtn);

            _itemListNode = GameObject.Find("UINode/onlineGameUI/itemlist/Viewport/Content").transform;
            showGameDataList();
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(GameLobbyMsgID.OnEnterOnlineGame,onEnterOnlineGame);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickCreateGameBtn,onClickCreateGameBtn);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnClickOnlineGameBtn,onClickOnlineGameBtn);
            _playManager = null;
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
            var datas = _playManager.GameDatas.Datas;
            for (int i = 0; i < datas.Length; i++)
            {
                Transform item_t = null;
                if(i<_itemListNode.childCount)
                {
                    item_t = _itemListNode.GetChild(i);
                }
                else
                {
                    item_t = GameObject.Instantiate(_itemListNode.GetChild(0).gameObject,_itemListNode).transform;
                }
                resetGameDataItem(datas[i],item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = datas.Length; i < _itemListNode.childCount; i++)
            {
                _itemListNode.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void resetGameDataItem(GameItemData data,Transform item_t)
        {
            item_t.GetChild(0).GetComponent<Image>().sprite = data.Icon;
            item_t.GetChild(1).GetComponent<Text>().text = data.Name;
        }

        private void showOnlineGameList()
        {
            var datas =new List<object>();
            for (int i = 0; i < datas.Count; i++)
            {
                Transform item_t = null;
                if(i<_itemListNode.childCount)
                {
                    item_t = _itemListNode.GetChild(i);
                }
                else
                {
                    item_t = GameObject.Instantiate(_itemListNode.GetChild(0).gameObject,_itemListNode).transform;
                }
                resetOnlineGameItem(datas[i],item_t);
                item_t.gameObject.SetActive(true);
            }
            for (int i = datas.Count; i < _itemListNode.childCount; i++)
            {
                _itemListNode.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void resetOnlineGameItem(object data,Transform item_t)
        {
            // item_t.GetChild(0).GetComponent<Image>().sprite = data.Icon;
            // item_t.GetChild(1).GetComponent<Text>().text = data.Name;
        }

        private void onClickOnlineGameBtn(object obj)
        {
            showOnlineGameList();
        }

        private void onClickCreateGameBtn(object obj)
        {
            showGameDataList();
        }

    }
}
