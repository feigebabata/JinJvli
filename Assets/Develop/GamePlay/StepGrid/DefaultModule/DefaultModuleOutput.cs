using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;
using UnityEngine.AddressableAssets;

namespace GamePlay.StepGrid
{
    public class DefaultModuleOutput : IDisposable
    {
        private StepGridPlayManager _playManager;
        private GridComp[] _gridComps;
        private StepGridConfig _stepGridConfig;
        private Coroutine _moveCor;
        private GameObject _startPanel,_stopPanel;
        private float _offsetY;

        public DefaultModuleOutput(StepGridPlayManager playManager)
        {
            _playManager = playManager;

            _playManager.Messenger.Add(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Add(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Add(StepGridMsgID.Stop,onPlayStop);
            _playManager.Messenger.Add(StepGridMsgID.Restart,onPlayRestart);
            loadStepGridConfig();
        }

        private async void loadStepGridConfig()
        {
            _stepGridConfig = await Addressables.LoadAssetAsync<StepGridConfig>("GamePlay.StepGrid.StepGridConfig").Task;
            initGrids();

            loadStartPanel();
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
            _offsetY = _stepGridConfig.OffsetLine * (_stepGridConfig.GridSize.y+_stepGridConfig.Spacing.y);
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
                yield return new WaitForEndOfFrame();
                _offsetY = -getMovingDistance(_stepGridConfig.StartSpeed,_stepGridConfig.Acceleration,Time.time-startTime) + _stepGridConfig.OffsetLine * (_stepGridConfig.GridSize.y+_stepGridConfig.Spacing.y);
                setGridsPos();
                checkGridIndex();
            }
        }

        void setGridsPos()
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                var pos = DefaultModule.GetGridPos(_gridComps[i].Index,_stepGridConfig.GridSize,_stepGridConfig.Spacing,_stepGridConfig.GridGroupWidth);
                _gridComps[i].transform.localPosition = new Vector3(pos.x,0,pos.y+_offsetY);
            }
        }

        void checkGridIndex()
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                if(_gridComps[i].transform.localPosition.z<_stepGridConfig.GridSize.y*-0.5f)
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

            girdComp.GetComponent<MeshRenderer>().material.color = DefaultModule.GridIsTarget(clickGrid.Index,_playManager.Module<DefaultModule>().GridListData,_stepGridConfig.GridGroupWidth)?_stepGridConfig.SelectCol:_stepGridConfig.ErrCol;
        }

        private void setGridColor(GridComp grid,GridListData gridListData)
        {
            Color color = _stepGridConfig.DefCol;
            if(DefaultModule.GridIsTarget(grid.Index,gridListData,_stepGridConfig.GridGroupWidth))
            {
                int line = grid.Index/_stepGridConfig.GridGroupWidth;
                // color = gridListData.LineColor[line%gridListData.LineColor.Length];
                int placeID = line%_playManager.GameStart.Players.Count;
                color = _stepGridConfig.Setas[placeID].Color;
            }
            grid.GetComponent<MeshRenderer>().material.color = color;
        }

        private void onPlayStop(object obj)
        {
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

        private void onPlayRestart(object obj)
        {
            _stopPanel.GetComponent<Canvas>().enabled=false;
            initGrids();
            _playManager.Messenger.Broadcast(StepGridMsgID.Start,null);
        }


    }
}
