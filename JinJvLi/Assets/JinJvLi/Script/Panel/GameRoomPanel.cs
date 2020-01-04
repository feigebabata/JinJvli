using System;
using System.Collections.Generic;
using System.IO;
using JinJvLi;
using JinJvLi.Lobby;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf;

namespace JinJvli
{
    [PanelConfig("Assets/JinJvLi/Res/JJL_Panel/GameRoomPanel.prefab",true)]
    public class GameRoomPanel: PanelBase
    {
        public static class Config
        {
            public const int MIN_UPDATE_TIME=2;
        }

        [SerializeField]
        UIList m_uiList;
        [SerializeField]
        Text m_titleText;
        [SerializeField]
        GameObject m_startBtn;
        List<PB_UserInfo> m_listData = new List<PB_UserInfo>();
        PB_GameRoom m_gameRoom;
        bool m_isStart;
        GameClinet m_client;
        PB_UserInfo m_userSelf;

        public override void OnCreate()
        {
            m_uiList.m_ItemShow += onItemShow;
            m_client = Main.Manager<NetworkManager>().Client<GameClinet>();
        }

        public override void OnShow(object _openData = null)
        {
            m_gameRoom = _openData as PB_GameRoom;
            m_titleText.text = $"{m_gameRoom.GameName} -- {m_gameRoom.Host.Name}";
            m_isStart = false;
            m_startBtn.SetActive(m_gameRoom.Host.Address.IP==NetworkManager.GetLocalIP().ToString());
            m_uiList.ItemNum=0;
            Broadcaster.Add<GameClinet.GameCmd>(onGameCmd);
            m_client.Connect(m_gameRoom.Address.IP,m_gameRoom.Address.Port);
            joinGame();
            base.OnShow();
        }

        public override void OnHide()
        {
            Broadcaster.Remove<GameClinet.GameCmd>(onGameCmd);
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
                Main.Manager<PanelManager>().ShowAlertDialog("退出房间将退出游戏!",()=>
                {
                    exitGame();
                    base.OnClickClose();
                });
            }
        }

        public void OnClickStart()
        {
            
        }

        void joinGame()
        {
            string user_json = PlayerPrefs.GetString(LoginPanel.Config.SELF_INFO);
            m_userSelf = PB_UserInfo.Parser.ParseJson(user_json);
            m_userSelf.Address = new PB_IPAddress(){IP=NetworkManager.GetLocalIP().ToString(),Port=m_client.Port};
            byte[] data = m_userSelf.ToByteArray();
            byte[] buffer = new byte[data.Length+NetworkManager.Config.NET_CMD_LENGTH];
            
            Array.Copy(BitConverter.GetBytes((UInt16)NetCmd.JoinGame),buffer,NetworkManager.Config.NET_CMD_LENGTH);
            Array.Copy(data,0,buffer,NetworkManager.Config.NET_CMD_LENGTH,data.Length);

            m_client.RedundancySend(buffer);
        }

        void exitGame()
        {
            byte[] data = m_userSelf.ToByteArray();
            byte[] buffer = new byte[data.Length+NetworkManager.Config.NET_CMD_LENGTH];
            
            Array.Copy(BitConverter.GetBytes((UInt16)NetCmd.ExitGame),buffer,NetworkManager.Config.NET_CMD_LENGTH);
            Array.Copy(data,0,buffer,NetworkManager.Config.NET_CMD_LENGTH,data.Length);

            m_client.RedundancySend(buffer);
        }

        void onGameCmd(GameClinet.GameCmd _gameCmd)
        {
            switch(_gameCmd.Cmd)
            {
                case NetCmd.JoinGame:
                    onJoinGame(_gameCmd.Buffer);
                break;
                case NetCmd.ExitGame:
                    onExitGame(_gameCmd.Buffer);
                break;
            }
        }

        void onJoinGame(byte[] _data)
        {
            PB_UserInfo userInfo=null;
            try
            {
                userInfo = PB_UserInfo.Parser.ParseFrom(_data);
                
            }
            catch{}
            if(userInfo != null)
            {
                addUIList(userInfo);
            }
        }

        void onExitGame(byte[] _data)
        {
            PB_UserInfo userInfo=null;
            try
            {
                userInfo = PB_UserInfo.Parser.ParseFrom(_data);
                
            }
            catch{}
            if(userInfo != null)
            {
                removeUIList(userInfo);
            }
        }

        void onItemShow(int _index, RectTransform _item)
        {
            _item.GetChild(0).GetComponent<Text>().text = m_listData[_index].Name;
        }

        void addUIList(PB_UserInfo _item)
        {
            var userInfo = m_listData.Find((_gr)=>{return _item.UID == _gr.UID;});
            if(userInfo == null)
            {
                if(_item.UID == m_userSelf.UID)
                {
                    m_userSelf = _item;
                }
                m_listData.Add(_item);
                m_listData.Sort((r1,r2)=>
                {
                    return r1.GameID - r2.GameID;
                });
                m_uiList.ItemNum=0;
                m_uiList.ItemNum = m_listData.Count;
            }
        }

        void removeUIList(PB_UserInfo _item)
        {
            var userInfo = m_listData.Find((_gr)=>{return _item.UID == _gr.UID;});
            if(userInfo == null)
            {
                m_listData.Remove(_item);
                m_listData.Sort((r1,r2)=>
                {
                    return r1.GameID - r2.GameID;
                });
                m_uiList.ItemNum=0;
                m_uiList.ItemNum = m_listData.Count;
            }
        }
    }
}