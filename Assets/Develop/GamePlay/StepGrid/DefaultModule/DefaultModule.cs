using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;

namespace GamePlay.StepGrid
{
    public class  DefaultModule : PlayModule<StepGridPlayManager>
    {
        private  DefaultModuleInput _moduleInput;
        private  DefaultModuleOutput _moduleOutput;
        public GridListData GridListData{get;private set;}
        private HashSet<int> _clickGrids = new HashSet<int>();

        public override void OnInit(PlayManager playManager)
        {
            if(IsInit)
            {
                return;
            }
            base.OnInit(playManager);
            //code
            _moduleInput = new  DefaultModuleInput(_playManager);
            _moduleOutput = new  DefaultModuleOutput(_playManager);

            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Add(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Add(StepGridMsgID.GridDestroy,onGridDestroy);
            _playManager.Messenger.Add(StepGridMsgID.Exit,onClickBack);

            GridListData = createGridListData(666);

        }

        public override void OnRelease()
        {
            if(!IsInit)
            {
                return;
            }
            //code

            GridListData=null;
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Remove(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Remove(StepGridMsgID.GridDestroy,onGridDestroy);
            _playManager.Messenger.Remove(StepGridMsgID.Exit,onClickBack);
            _moduleInput.Dispose();
            _moduleOutput.Dispose();
            base.OnRelease();
        }

        public override void OnShow()
        {
            base.OnShow();
            //code
            
        }

        public override void OnHide()
        {
            base.OnHide();
            //code
        }

        private void onClickBack(object data)
        {
            _playManager.Destroy();
            new GameLobby.GameLobbyPlayManager().Create();
        }

        /// <summary>
        /// 格子位置 格子顺序:左→右 下→上 左右居中
        /// </summary>
        /// <param name="index">格子顺序索引</param>
        /// <param name="grid">格子大小</param>
        /// <param name="spacing">格子间隔</param>
        /// <param name="width">横向格子数</param>
        /// <returns></returns>
        public static Vector2 GetGridPos(int index,Vector2 grid,Vector2 spacing,int width)
        {
            Vector2 gridPos=Vector2.zero;
            float groupWidth = grid.x*width+spacing.x*(width-1);
            int line_idx = index%width;
            gridPos.x = line_idx*spacing.x+line_idx*grid.x-groupWidth/2+grid.x/2;
            int line = index/width;
            gridPos.y = line*(grid.y+spacing.y)+grid.y/2;
            return gridPos;
        }

        GridListData createGridListData(int seed)
        {
            var data = new GridListData();
            data.GridValues = new byte[1024];

            UnityEngine.Random.InitState(seed);
            for (int i = 0; i < data.GridValues.Length; i++)
            {
                data.GridValues[i] = (byte)UnityEngine.Random.Range(0,4);
            }

            data.LineColor = new Color[]{Color.yellow};
            return data;
        }

        public static bool GridIsTarget(int index,GridListData gridListData,int gridGroupWidth)
        {
            int line = index/gridGroupWidth;
            int x_i = index%gridGroupWidth;
            int val = gridListData.GridValues[line%gridListData.GridValues.Length];
            return val==x_i;
        }

        private void onClickGrid(object obj)
        {
            int index = (int)obj;
            if(!GridIsTarget(index,GridListData,4))
            {
                _playManager.Messenger.Broadcast(StepGridMsgID.Stop,null);
            }
            _clickGrids.Add(index);
        }

        private void onGridDestroy(object obj)
        {
            int index = (int)obj;
            if(GridIsTarget(index,GridListData,4) && !_clickGrids.Contains(index))
            {
                _playManager.Messenger.Broadcast(StepGridMsgID.Stop,null);
            }
            _clickGrids.Remove(index);
        }


    }

    public class GridListData
    {
        public byte[] GridValues;
        public Color[] LineColor;
    }
}
