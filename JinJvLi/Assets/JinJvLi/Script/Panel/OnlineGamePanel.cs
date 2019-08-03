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
        [SerializeField]
        UIList m_uiList;
        List<PB_GameRoom> m_listData = new List<PB_GameRoom>();
        PB_GameRoom m_curSelect;
        Coroutine m_updateList;

        public override void OnOpen(object _openData = null)
        {
            m_uiList.m_ItemShow += onItemShow;
            m_uiList.ItemNum=0;
        }

        public override void OnShow()
        {
            m_updateList = Coroutines.Inst.LoopRun(1,-1,updateList);
            base.OnShow();
        }

        public override void OnHide()
        {
            Coroutines.Inst.Stop(m_updateList);
            base.OnHide();
        }

        public void OnClickCreate()
        {

        }

        public void OnClickJoin()
        {
            
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

        }

        void addUIList(PB_GameRoom _gameRoom)
        {
            m_listData.Add(_gameRoom);
            m_listData.Sort((r1,r2)=>
            {
                return r1.CreateTime - r2.CreateTime;
            });
        }
    }
}