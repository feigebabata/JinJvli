using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.StepGrid
{
    public class DefaultModuleInput : IDisposable
    {
        StepGridPlayManager _playManager;
        StartPanelComps _startPanelComps;

        public DefaultModuleInput(StepGridPlayManager playManager)
        {
            _playManager = playManager;
            _playManager.Messenger.Add(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Add(StepGridMsgID.Stop,onPlayStop);
            _playManager.Messenger.Add(StepGridMsgID.PanelLoadComplete,onPanelLoadComplete);
        }

        public void Dispose()
        {
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
                        _playManager.Messenger.Broadcast(StepGridMsgID.ClickGrid,gridComp.Index);
                    }
                }
            }
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
        }

        private void onClickStartBtn()
        {
            _playManager.Messenger.Broadcast(StepGridMsgID.Start,null);
        }
    }
}
