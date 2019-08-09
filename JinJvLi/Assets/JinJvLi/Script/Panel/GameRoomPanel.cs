using System;
using System.Collections.Generic;
using System.IO;
using JinJvLi;
using JinJvLi.Lobby;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JinJvli
{
    [PanelConfig("JJL_Panel/GameRoomPanel")]
    public class GameRoomPanel: PanelBase
    {
        public static class Config
        {
            public const int MIN_UPDATE_TIME=2;
        }

        public class UserInfoItem
        {
            public PB_UserInfo UserInfo;
            public float CreateTime;
            public float UpdateTime;
        }

        [SerializeField]
        UIList m_uiList;
        [SerializeField]
        TMP_Text m_titleText;
        List<UserInfoItem> m_listData = new List<UserInfoItem>();
        Coroutine m_updateList;
        List<UserInfoItem> m_remove = new List<UserInfoItem>();
        PB_GameRoom m_gameRoom;
        bool m_isStart;

        public override void OnCreate(object _openData = null)
        {
            m_gameRoom = _openData as PB_GameRoom;
            m_uiList.m_ItemShow += onItemShow;
            m_titleText.text = m_gameRoom.GameName;
        }

        public override void OnShow()
        {
            m_isStart = false;
            m_uiList.ItemNum=0;
            m_updateList = Coroutines.Inst.LoopRun(1,-1,updateList);
            Broadcaster.Add<NetworkManager.NetBroadcast>(onNetBroadcast);
            // GameServer gameServer = Main.Manager<NetworkManager>().CreateServer<GameServer>();
            // PB_GameRoom gameRoom = new PB_GameRoom();
            // gameRoom.GameName="会跳舞的线";
            // string user_json = PlayerPrefs.GetString(LoginPanel.Config.SELF_INFO);
            // gameRoom.Host = PB_UserInfo.Parser.ParseJson(user_json);
            // gameServer.Start(gameRoom,NetCmd.GameRoom);
            base.OnShow();
            Main.Manager<PanelManager>().ShowToast("游戏已创建 等待其他玩家加入和你的开始",5);
        }

        public override void OnHide()
        {
            Broadcaster.Remove<NetworkManager.NetBroadcast>(onNetBroadcast);
            Coroutines.Inst.Stop(m_updateList);
            base.OnHide();
        }

        public override void OnClickClose()
        {
            if(m_isStart)
            {
                base.OnClickClose();
            }
            else
            {
                Main.Manager<PanelManager>().ShowAlertDialog("退出房间将关闭游戏!",()=>
                {
                    base.OnClickClose();
                });
            }
        }

        void Update()
        {
            updateList();
        }

        public void OnClickStart()
        {
            
        }

        void onNetBroadcast(NetworkManager.NetBroadcast _netData)
        {
            if(_netData.Cmd == NetCmd.GameRoom)
            {
                UserInfoItem userInfo=null;
                try
                {
                    userInfo.UserInfo = PB_UserInfo.Parser.ParseFrom(_netData.Buffer);
                    
                }
                catch{}
                if(userInfo != null)
                {
                    userInfo.CreateTime= Time.time;
                    userInfo.UpdateTime= Time.time;
                    addUIList(userInfo);
                }
            }
        }

        void onItemShow(int _index, RectTransform _item)
        {
            _item.GetChild(0).GetComponent<Text>().text = m_listData[_index].UserInfo.Name;
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

        void addUIList(UserInfoItem _item)
        {
            var userInfo = m_listData.Find((_gr)=>{return _item.UserInfo.UID == _gr.UserInfo.UID;});
            if(userInfo == null)
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
                userInfo.UpdateTime = Time.time;
            }
        }
    }
}