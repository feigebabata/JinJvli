using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace GamePlay.GameLobby
{
    public class LobbyModuleOutput : IDisposable
    {
        GameLobbyPlayManager _playManager;
        private Transform _gameItemsParent;
        private Color _select = new Color32(255,0,255,255);
        private Color _unSelect = new Color32(0,255,255,255);

        public LobbyModuleOutput(GameLobbyPlayManager playManager)
        {
            _playManager = playManager;
            _gameItemsParent = GameObject.Find("gamelist").transform;
            
            _playManager.Messenger.Add(GameLobbyMsgID.OnUnSelectGameItem,onUnSelectGameItem);
            _playManager.Messenger.Add(GameLobbyMsgID.OnSelectGameItem,onSelectGameItem);
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(GameLobbyMsgID.OnUnSelectGameItem,onUnSelectGameItem);
            _playManager.Messenger.Remove(GameLobbyMsgID.OnSelectGameItem,onSelectGameItem);

            _playManager = null;
        }

        public IEnumerator ShowItemList()
        {
            var loader = Addressables.LoadAssetAsync<GameItemDatas>("GamePlay.GameLobby.GameDatas");
            yield return loader;
            var datas = loader.Result.Datas;
            bool ignore = false;
            for (int i = 0; i < datas.Length; i++)
            {
                if(!Application.isEditor && Application.platform==RuntimePlatform.Android)
                {
                    ignore = !datas[i].AndroidPlatform;
                }
                else if(!Application.isEditor && Application.platform==RuntimePlatform.WebGLPlayer)
                {
                    ignore = !datas[i].WebPlatform;
                }
                else if(!Application.isEditor)
                {
                    ignore = !datas[i].PCPlatform;
                }

                if(!ignore)
                {
                    Transform item = GameObject.Instantiate(_gameItemsParent.GetChild(0),_gameItemsParent,false);
                    item.GetComponent<GameItem>().TypeName = datas[i].TypeName;
                    item.GetChild(0).GetComponent<SpriteRenderer>().sprite = datas[i].Icon;
                    item.GetChild(0).localScale = datas[i].Scale;
                    var space = item.localPosition*(1+i*0.02f);
                    item.localPosition = Quaternion.Euler(0,23*i,0) * space;
                    item.gameObject.SetActive(true);
                    item.name = datas[i].TypeName;
                }
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

    }
}