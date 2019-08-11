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

        public class GameRoomItem
        {
            public PB_GameRoom GameRoom;
            public float CreateTime;
            public float UpdateTime;
        }

        [SerializeField]
        UIList m_uiList;
        List<GameRoomItem> m_listData = new List<GameRoomItem>();
        GameRoomItem m_curSelect;
        Coroutine m_updateList;
        List<GameRoomItem> m_remove = new List<GameRoomItem>();

        public override void OnCreate()
        {
            m_uiList.m_ItemShow += onItemShow;
        }

        public override void OnShow(object _openData = null)
        {
            m_curSelect=null;
            m_uiList.ItemNum=0;
            m_updateList = Coroutines.Inst.LoopRun(1,-1,updateList);
            Broadcaster.Add<NetworkManager.NetBroadcast>(onNetBroadcast);
            base.OnShow();
        }

        public override void OnHide()
        {
            Broadcaster.Remove<NetworkManager.NetBroadcast>(onNetBroadcast);
            m_updateList.Stop();
            base.OnHide();
        }

        public void OnClickCreate()
        {
            Main.Manager<PanelManager>().Open<CreateGamePanel>();
        }

        public void OnClickJoin()
        {
            if(m_curSelect != null)
            {
                 Main.Manager<PanelManager>().Open<GameRoomPanel>(m_curSelect.GameRoom);
            }
        }

        void onNetBroadcast(NetworkManager.NetBroadcast _netData)
        {
            if(_netData.Cmd == NetCmd.GameRoom)
            {
                GameRoomItem gameRoom=new GameRoomItem();
                try
                {
                    gameRoom.GameRoom = PB_GameRoom.Parser.ParseFrom(_netData.Buffer);
                    
                }
                catch{}
                if(gameRoom.GameRoom != null)
                {
                    gameRoom.CreateTime= Time.time;
                    gameRoom.UpdateTime= Time.time;
                    addUIList(gameRoom);
                }
            }
        }

        void onItemShow(int _index, RectTransform _item)
        {
            _item.GetChild(0).GetComponent<Text>().text = $"[{m_listData[_index].GameRoom.GameName}]{m_listData[_index].GameRoom.Tip}";
            _item.GetChild(1).GetComponent<Text>().text = $"<color={m_listData[_index].GameRoom.Host.Color}>{m_listData[_index].GameRoom.Host.Name}</color>";
            _item.GetChild(2).gameObject.SetActive(false);
            _item.GetComponent<Button>().onClick.RemoveAllListeners();
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

        void addUIList(GameRoomItem _item)
        {
            var gameRoom = m_listData.Find((_gr)=>{return _item.GameRoom.ID == _gr.GameRoom.ID && _item.GameRoom.Host.UID == _gr.GameRoom.Host.UID ;});
            if(gameRoom == null)
            {
                _item.UpdateTime = Time.time;
                m_listData.Add(_item);
                m_listData.Sort((r1,r2)=>
                {
                    return (int)(r1.CreateTime - r2.CreateTime);
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