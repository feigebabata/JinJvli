using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;

namespace GamePlay.StepGrid
{
    public class  DefaultModule : PlayModule<StepGridPlayManager>
    {
        private  DefaultModuleInput _moduleInput;
        private  DefaultModuleOutput _moduleOutput;
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

            initGrids();
        }

        public override void OnRelease()
        {
            if(!IsInit)
            {
                return;
            }
            //code

            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
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
        Vector2 getGridPos(int index,Vector2 grid,Vector2 spacing,int width)
        {
            Vector2 gridPos=Vector2.zero;
            float groupWidth = grid.x*width+spacing.x*(width-1);
            int line_idx = index%width;
            gridPos.x = line_idx*spacing.x+line_idx*grid.x-groupWidth/2+grid.x/2;
            int line = index/width;
            gridPos.y = line*(grid.y+spacing.y)+grid.y/2;
            return gridPos;
        }

        void initGrids()
        {
            Transform gridsT = GameObject.Find("grids").transform;
            Vector2 gridSize = new Vector2(1,2);
            Vector2 spacing = new Vector2(0.1f,0.1f);
            for (int i = 0; i < gridsT.childCount; i++)
            {
                var pos = getGridPos(i,gridSize,spacing,4);
                gridsT.GetChild(i).localPosition = new Vector3(pos.x,0,pos.y);
            }
        }

    }
}
