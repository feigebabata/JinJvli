using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using Google.Protobuf;

namespace GamePlay.StepGrid
{
    public class DefaultModuleInput : IDisposable
    {
        private StepGridPlayManager _playManager;
        private StartPanelComps _startPanelComps;
        private StopPanelComps _stopPanelComps;

        public DefaultModuleInput(StepGridPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Add(StepGridMsgID.Stop,onPlayStop);
            _playManager.Messenger.Add(StepGridMsgID.PanelLoadComplete,onPanelLoadComplete);
            _playManager.NetworkSyncSystem.Messenger.Add((ushort)StepGridMsgID.ClickGrid,onClickGrid);
        }

        public void Dispose()
        {
            MonoBehaviourEvent.I.UpdateListener -= Update;
            _playManager.Messenger.Remove(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Remove(StepGridMsgID.Stop,onPlayStop);
            _playManager.Messenger.Remove(StepGridMsgID.PanelLoadComplete,onPanelLoadComplete);
            _startPanelComps.StartBtn.onClick.RemoveAllListeners();
            _startPanelComps=null;
            _playManager = null;
        }

        void initGrids()
        {
            Transform gridsT = GameObject.Find("grids").transform;
            for (int i = 0; i < gridsT.childCount; i++)
            {
                gridsT.GetChild(i).gameObject.AddComponent<BoxCollider>();
                
            }
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if(Physics.Raycast(ray,out raycastHit))
                {
                    GridComp gridComp = raycastHit.transform.GetComponent<GridComp>();
                    if(gridComp)
                    {
                        sendClickMsg(gridComp.Index);
                    }
                }
            }
        }

        void sendClickMsg(int gridIndex)
        {
            var clickGrid = new PB_ClickGrid()
            {
                Index = gridIndex,
                PlaceIndex = _playManager.SelfInfo.PlaceIndex,
            };
            _playManager.NetworkSyncSystem.SendMsg((uint)StepGridMsgID.ClickGrid,_playManager.GamePlayID,clickGrid);
        }

        private void onPlayStop(object obj)
        {
            MonoBehaviourEvent.I.UpdateListener -= Update;
        }

        private void onPlayStart(object obj)
        {
            initGrids();
            MonoBehaviourEvent.I.UpdateListener += Update;
        }

        private void onPanelLoadComplete(object obj)
        {
            var go = obj as GameObject;
            if(go.name=="StartPanel")
            {
                _startPanelComps = go.GetComponent<StartPanelComps>();
                _startPanelComps.StartBtn.onClick.AddListener(onClickStartBtn);
            }
            else if(go.name=="StopPanel")
            {
                _stopPanelComps = go.GetComponent<StopPanelComps>();
                _stopPanelComps.RestartBtn.onClick.AddListener(onClickRestartBtn);
                _stopPanelComps.ExitBtn.onClick.AddListener(onClickExitBtn);
                _stopPanelComps.Tip.text = "游戏结束";
            }
        }

        private void onClickExitBtn()
        {
            _playManager.Messenger.Broadcast(StepGridMsgID.Exit,null);
        }

        private void onClickStartBtn()
        {
            _playManager.Messenger.Broadcast(StepGridMsgID.Start,null);
        }

        private void onClickRestartBtn()
        {
            _playManager.Messenger.Broadcast(StepGridMsgID.Restart,null);
        }

        private void onClickGrid(ByteString obj)
        {
            var clickGrid = PB_ClickGrid.Parser.ParseFrom(obj);
            _playManager.Messenger.Broadcast(StepGridMsgID.ClickGrid,clickGrid);
        }
    }
}
