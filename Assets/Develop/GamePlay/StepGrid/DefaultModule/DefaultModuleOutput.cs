using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.StepGrid
{
    public class DefaultModuleOutput : IDisposable
    {
        StepGridPlayManager _playManager;
        GridComp[] _gridComps;
        Vector2 _gridSize = new Vector2(1,3);
        Vector2 _spacing = new Vector2(0.1f,0.2f);
        int _gridGroupWidth = 4;
        float _acceleration=0.1f;
        float _startSpeed=2;
        float _offsetY;
        Color _defCol = Color.white;
        Color _selectCol = Color.green;
        Color _errCol = Color.red;
        private Coroutine _moveCor;
        private StartPanel _startPanel;

        public DefaultModuleOutput(StepGridPlayManager playManager)
        {
            _playManager = playManager;

            _playManager.Messenger.Add(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Add(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Add(StepGridMsgID.Stop,onPlayStop);
            _startPanel = new StartPanel();
            initGrids();

            loadPanel().Start();
        }

        public void Dispose()
        {
            _playManager.Messenger.Remove(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Remove(StepGridMsgID.Start,onPlayStart);
            _playManager.Messenger.Remove(StepGridMsgID.Stop,onPlayStop);

            _playManager.UISystem.ReleaseView(_startPanel);
            _startPanel=null;

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
            _offsetY = 4 * (_gridSize.y+_spacing.y);
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
                _offsetY = -getMovingDistance(_startSpeed,_acceleration,Time.time-startTime) + 4 * (_gridSize.y+_spacing.y);
                setGridsPos();
                checkGridIndex();
            }
        }

        void setGridsPos()
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                var pos = DefaultModule.GetGridPos(_gridComps[i].Index,_gridSize,_spacing,_gridGroupWidth);
                _gridComps[i].transform.localPosition = new Vector3(pos.x,0,pos.y+_offsetY);
            }
        }

        void checkGridIndex()
        {
            for (int i = 0; i < _gridComps.Length; i++)
            {
                if(_gridComps[i].transform.localPosition.z<_gridSize.y*-0.5f)
                {
                    _playManager.Messenger.Broadcast(StepGridMsgID.GridDestroy,_gridComps[i].Index);
                    _gridComps[i].Index+=_gridComps.Length;
                    setGridColor(_gridComps[i],_playManager.Module<DefaultModule>().GridListData);
                }
            }
        }

        private void onClickGrid(object obj)
        {
            int index = (int)obj;
            var girdComp = Array.Find<GridComp>(_gridComps,(g)=>
            {
                return g.Index == index;
            });

            girdComp.GetComponent<MeshRenderer>().material.color = DefaultModule.GridIsTarget(index,_playManager.Module<DefaultModule>().GridListData,_gridGroupWidth)?_selectCol:_errCol;
        }

        private void setGridColor(GridComp grid,GridListData gridListData)
        {
            Color color = _defCol;
            if(DefaultModule.GridIsTarget(grid.Index,gridListData,_gridGroupWidth))
            {
                int line = grid.Index/_gridGroupWidth;
                color = gridListData.LineColor[line%gridListData.LineColor.Length];
            }
            grid.GetComponent<MeshRenderer>().material.color = color;
        }

        private void onPlayStop(object obj)
        {
            _moveCor.Stop();
        }

        private void onPlayStart(object obj)
        {

            for (int i = 0; i < _gridComps.Length; i++)
            {
                setGridColor(_gridComps[i],_playManager.Module<DefaultModule>().GridListData);
            }

            _moveCor = moveGrid().Start();

            _playManager.UISystem.HideView(_startPanel);
        }

        IEnumerator loadPanel()
        {
           yield return _playManager.UISystem.CreateView(_startPanel,_playManager);
           _playManager.UISystem.ShowView(_startPanel);
           SceneLoading.I.Hide();
        }

    }
}
