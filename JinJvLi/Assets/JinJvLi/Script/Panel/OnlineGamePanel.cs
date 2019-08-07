using System;
using System.Collections.Generic;
using System.IO;
using JinJvLi;
using JinJvLi.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace JinJvli
{
    [PanelConfig("JJL_Panel/OnlineGamePanel")]
    public class OnlineGamePanel: PanelBase
    {
        public static class Config
        {
            public const int MIN_UPDATE_TIME=2;
        }

        [SerializeField]
        UIList m_uiList;
        List<PB_GameRoom> m_listData = new List<PB_GameRoom>();
        PB_GameRoom m_curSelect;
        Coroutine m_updateList;
        List<PB_GameRoom> m_remove = new List<PB_GameRoom>();

        public override void OnCreate(object _openData = null)
        {
            m_uiList.m_ItemShow += onItemShow;
            m_uiList.ItemNum=0;
        }

        public override void OnShow()
        {
            Broadcaster.Add<NetworkManager.NetBroadcast>(onNetBroadcast);
            m_updateList = Coroutines.Inst.LoopRun(1,-1,updateList);
            GameServer gameServer = Main.Manager<NetworkManager>().CreateServer<GameServer>();
            PB_GameRoom gameRoom = new PB_GameRoom();
            gameRoom.GameName="会跳舞的线";
            string user_json = PlayerPrefs.GetString(LoginPanel.Config.SELF_INFO);
            gameRoom.Host = PB_UserInfo.Parser.ParseJson(user_json);
            // gameServer.Start(gameRoom,NetCmd.GameRoom);
            base.OnShow();
        }

        public override void OnHide()
        {
            Coroutines.Inst.Stop(m_updateList);
            base.OnHide();
        }

        void Update()
        {
            updateList();
        }

        public void OnClickCreate()
        {

        }

        public void OnClickJoin()
        {
            if(m_curSelect != null)
            {
                
            }
        }

        void onNetBroadcast(NetworkManager.NetBroadcast _netData)
        {
            if(_netData.Cmd == NetCmd.GameRoom)
            {
                PB_GameRoom gameRoom=null;
                try
                {
                    gameRoom = PB_GameRoom.Parser.ParseFrom(_netData.Buffer);
                }
                catch{}
                if(gameRoom != null)
                {
                    addUIList(gameRoom);
                }
            }
        }

        void onItemShow(int _index, RectTransform _item)
        {
            _item.GetChild(0).GetComponent<Text>().text = $"[{m_listData[_index].GameName}]{m_listData[_index].Tip}";
            _item.GetChild(1).GetComponent<Text>().text = $"<color={m_listData[_index].Host.Color}>{m_listData[_index].Host.Name}</color>";
            _item.GetChild(2).gameObject.SetActive(false);
            _item.GetComponent<Button>().onClick.AddListener(()=>
            {
                selectItem(_index,_item.GetChild(2).gameObject);
            });
        }

        void selectItem(int _index,GameObject _select)
        {
            if(_select.activeSelf)
            {
                m_curSelect=null;
                _select.SetActive(false);
            }
            else
            {
                if(m_curSelect!= null)
                {
                    return;
                }
                m_curSelect=m_listData[_index];
                _select.SetActive(true);
            }
        }

        void updateList()
        {
            m_remove.Clear();
            for (int i = 0; i < m_listData.Count; i++)
            {
                if(Time.time - m_listData[i].UpdateTime>Config.MIN_UPDATE_TIME)
                {
                    m_remove.Add(m_listData[i]);
                }
            }
            for (int i = 0; i < m_remove.Count; i++)
            {
                m_listData.Remove(m_remove[i]);
            }
            if(m_remove.Count>0)
            {
                m_uiList.ItemNum=0;
                m_uiList.ItemNum=m_listData.Count;
            }
        }

        void addUIList(PB_GameRoom _gameRoom)
        {
            var gameRoom = m_listData.Find((_gr)=>{return _gameRoom.ID == _gr.ID && _gameRoom.Host.UID == _gr.Host.UID ;});
            if(gameRoom == null)
            {
                _gameRoom.UpdateTime = Time.time;
                m_listData.Add(_gameRoom);
                m_listData.Sort((r1,r2)=>
                {
                    return r1.CreateTime - r2.CreateTime;
                });
                m_uiList.ItemNum=0;
                m_uiList.ItemNum = m_listData.Count;
            }
            else
            {
                gameRoom.UpdateTime = Time.time;
            }
        }
    }
}