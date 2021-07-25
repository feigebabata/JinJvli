using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;
using System.Text;

namespace GamePlay.StepGrid
{
    public class DefaultModuleOutput : IDisposable
    {
        private StepGridPlayManager _playManager;
        private GridComp[] _gridComps;
        private Coroutine _moveCor;
        private GameObject _startPanel,_stopPanel;
        private PlayPanelComps _playPanelComps;
        private float _offsetY;
        private Coroutine _resetReadyState;

        public DefaultModuleOutput(StepGridPlayManager playManager)
        {
            _playManager = playManager;

            _playManager.Messenger.Add(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Add(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Add(StepGridMsgID.Stop,onPlayStop);
            _playManager.Messenger.Add(StepGridMsgID.Restart,onPlayRestart);
            
            initGrids();

            loadStartPanel();
            loadPlayPanel();
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Remove(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Remove(StepGridMsgID.Stop,onPlayStop);
            _playManager.Messenger.Remove(StepGridMsgID.Restart,onPlayRestart);

            _startPanel?.GetComponent<Canvas>().ClearSortOrder();
            GameObject.Destroy(_startPanel);
            _stopPanel?.GetComponent<Canvas>().ClearSortOrder();
            GameObject.Destroy(_stopPanel);
            _moveCor?.Stop();
            _moveCor=null;
            _playManager = null;
        }

        void initGrids()
        {
            Transform gridsT = GameObject.Find("grids").transform;
            _gridComps = new GridComp[gridsT.childCount];
            for (int i = 0; i < gridsT.childCount; i++)
            {
                _gridComps[i] = gridsT.GetChild(i).GetComponent<GridComp>();
                _gridComps[i].Index = i;
            }
            _offsetY = _playManager.StepGridConfig.OffsetLine * (_playManager.StepGridConfig.GridSize.y+_playManager.StepGridConfig.Spacing.y);
            setGridsPos();
            
            
        }


        /// <summary>
        /// 匀加速位移
        /// </summary>
        /// <param name="startSpeed">起始速度</param>
        /// <param name="acceleration">加速度</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        float getMovingDistance(float startSpeed,float acceleration,float time)
        {
            return startSpeed*time+acceleration*time*time/2;
        }

        IEnumerator moveGrid()
        {
            float startTime = Time.time;
            while (true)
            {
                _offsetY = -getMovingDistance(_playManager.StepGridConfig.StartSpeed,_playManager.StepGridConfig.Acceleration,Time.time-startTime) + _playManager.StepGridConfig.OffsetLine * (_playManager.StepGridConfig.GridSize.y+_playManager.StepGridConfig.Spacing.y);
                setGridsPos();
                checkGridIndex();
                yield return new WaitForEndOfFrame();
            }
        }

        void setGridsPos()
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                var pos = DefaultModule.GetGridPos(_gridComps[i].Index,_playManager.StepGridConfig.GridSize,_playManager.StepGridConfig.Spacing,_playManager.StepGridConfig.GridGroupWidth);
                _gridComps[i].transform.localPosition = new Vector3(pos.x,0,pos.y+_offsetY);
            }
        }

        void checkGridIndex()
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                if(_gridComps[i].transform.localPosition.z<_playManager.StepGridConfig.GridSize.y*-0.5f)
                {
                    _playManager.Messenger.Broadcast(StepGridMsgID.GridDestroy,_gridComps[i].Index);
                    _gridComps[i].Index+=_gridComps.Length;
                    setGridColor(_gridComps[i],_playManager.Module<DefaultModule>().GridListData);
                }
            }
        }

        private void onClickGrid(object obj)
        {
            PB_ClickGrid clickGrid = obj as PB_ClickGrid;
            var girdComp = Array.Find<GridComp>(_gridComps,(g)=>
            {
                return g.Index == clickGrid.Index;
            });

            girdComp.GetComponent<MeshRenderer>().material.color = DefaultModule.GridIsTarget(clickGrid.Index,_playManager.Module<DefaultModule>().GridListData,_playManager.StepGridConfig.GridGroupWidth)?_playManager.StepGridConfig.SelectCol:_playManager.StepGridConfig.ErrCol;
        }

        private void setGridColor(GridComp grid,GridListData gridListData)
        {
            Color color = _playManager.StepGridConfig.DefCol;
            if(DefaultModule.GridIsTarget(grid.Index,gridListData,_playManager.StepGridConfig.GridGroupWidth))
            {
                int placeID = DefaultModule.Index2PlaceID(grid.Index,_playManager.StepGridConfig.GridGroupWidth,_playManager.GameStart.Players.Count);
                color = _playManager.StepGridConfig.Setas[placeID].Color;
            }
            grid.GetComponent<MeshRenderer>().material.color = color;
        }

        private void onPlayStop(object obj)
        {
            int placeID = (int)obj;
            Debug.LogWarning(_playManager.GameStart.Players[placeID].PlayerInfo.Nickname+" 输了");
            _moveCor.Stop();
            _moveCor=null;
            loadStopPanel();
        }

        private void onPlayStart(object obj)
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                setGridColor(_gridComps[i],_playManager.Module<DefaultModule>().GridListData);
            }

            _moveCor = moveGrid().Start();
            _startPanel.GetComponent<Canvas>().enabled = false;
            _resetReadyState?.Stop();
            _resetReadyState=null;
            resetPlayPanel().Start();
        }

        async void loadStartPanel()
        {
            _startPanel = await Addressables.InstantiateAsync("GamePlay.StepGrid.DefaultModule.StartPanel",null,false).Task;
            _startPanel.GetComponent<Canvas>().SetPanelSortOrder();
            _startPanel.name = "StartPanel";
            _playManager.Messenger.Broadcast(StepGridMsgID.PanelLoadComplete,_startPanel);
            SceneLoading.I.Hide();
        }

        async void loadStopPanel()
        {
            _stopPanel = await Addressables.InstantiateAsync("GamePlay.StepGrid.DefaultModule.StopPanel",null,false).Task;
            _stopPanel.GetComponent<Canvas>().SetPanelSortOrder();
            _stopPanel.name = "StopPanel";
            _playManager.Messenger.Broadcast(StepGridMsgID.PanelLoadComplete,_stopPanel);
        }

        async void loadPlayPanel()
        {
            var go = await Addressables.InstantiateAsync("GamePlay.StepGrid.DefaultModule.PlayPanel",null,false).Task;
            go.name = "PlayPanel";
            _playPanelComps = go.GetComponent<PlayPanelComps>();
            _resetReadyState=resetReadyState().Start();
        }

        private void onPlayRestart(object obj)
        {
            _stopPanel.GetComponent<Canvas>().enabled=false;
            initGrids();
            _playManager.Messenger.Broadcast(StepGridMsgID.Start,null);
        }

        private IEnumerator resetPlayPanel()
        {
            StringBuilder sb=new StringBuilder();
            foreach (var player in _playManager.GameStart.Players)
            {
                sb.AppendLine($"{_playManager.StepGridConfig.Setas[player.PlaceIndex].Color.RichText(player.PlayerInfo.Nickname)}");
            }
            _playPanelComps.Text.text = sb.ToString();
            yield break;
        }

        private IEnumerator resetReadyState()
        {
            
            StringBuilder sb=new StringBuilder();
            while (true)
            {
                var readys = _playManager.Module<DefaultModule>().GameReadys;
                for (int i = 0; i < readys.Length; i++)
                {
                    var player = _playManager.GameStart.Players[i];
                    if(readys[i])
                    {
                        sb.AppendLine($"{_playManager.StepGridConfig.Setas[player.PlaceIndex].Color.RichText(player.PlayerInfo.Nickname)} 已准备");  
                    }
                    else
                    {
                        sb.AppendLine($"{_playManager.StepGridConfig.Setas[player.PlaceIndex].Color.RichText(player.PlayerInfo.Nickname)} 未准备");  
                    } 
                }
                _playPanelComps.Text.text = sb.ToString();
                sb.Clear();
                yield return new WaitForSeconds(1);
            }
        }


    }
}
