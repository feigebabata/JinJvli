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

        public DefaultModuleOutput(StepGridPlayManager playManager)
        {
            _playManager = playManager;

            initGrids();
        }
        
        public void Dispose()
        {
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

            moveGrid().Start();
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
                if(_gridComps[i].transform.localPosition.z<_gridSize.y*-0.5f+_spacing.y)
                {
                    _gridComps[i].Index+=_gridComps.Length;
                }
            }
        }

    }
}
