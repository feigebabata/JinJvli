using System;
using System.Collections.Generic;
using System.IO;
using JinJvLi;
using JinJvLi.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace JinJvli
{
    [PanelConfig("JJL_Panel/CreateGamePanel")]
    public class CreateGamePanel: PanelBase
    {
        public static class Config
        {
            public const string GAME_INFO_LIST_PATH="GameInfoList";
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
            base.OnShow();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public void OnClickCreate()
        {
            if(m_curSelect != null)
            {
                
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
    }

    [Serializable]
    public class GameInfo
    {
        public string Name;
        public UInt32 ID;
        public string Tip;
    } 

    [CreateAssetMenu(menuName="GameInfoList")]
    public class GameInfoList : ScriptableObject
    {
        public GameInfo[] List;
    }
}