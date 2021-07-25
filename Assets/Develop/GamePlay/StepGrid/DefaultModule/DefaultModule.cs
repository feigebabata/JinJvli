using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FGUFW.Core;
using FGUFW.Play;
using System;
using Google.Protobuf;

namespace GamePlay.StepGrid
{
    public class  DefaultModule : PlayModule<StepGridPlayManager>
    {
        private  DefaultModuleInput _moduleInput;
        private  DefaultModuleOutput _moduleOutput;
        public GridListData GridListData{get;private set;}
        private HashSet<int> _clickGrids = new HashSet<int>();
        public bool[] GameReadys;
        private object _gameReadyLock = new object();
        private int _frameIndex=0;


        public DefaultModule(PlayManager playManager) : base(playManager)
        {
            _moduleInput = new  DefaultModuleInput(_playManager);
            _moduleOutput = new  DefaultModuleOutput(_playManager);
            UdpBroadcastUtility.OnReceive+=onGameReadyReceive;

            GlobalMessenger.M.Add(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Add(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Add(StepGridMsgID.GridDestroy,onGridDestroy);
            _playManager.Messenger.Add(StepGridMsgID.Exit,onClickBack);
            _playManager.Messenger.Add(StepGridMsgID.Restart,onClickRestart);
            _playManager.Messenger.Add(StepGridMsgID.Stop,onGameStop);

            GridListData = createGridListData(666);
            GameReadys = new bool[_playManager.GameStart.Players.Count];
            MonoBehaviourEvent.I.UpdateListener+=Update;
        }

        public override void Dispose()
        {
            MonoBehaviourEvent.I.UpdateListener-=Update;
            UdpBroadcastUtility.OnReceive-=onGameReadyReceive;
            GridListData=null;
            GlobalMessenger.M.Remove(GlobalMsgID.OnBackKey,onClickBack);
            _playManager.Messenger.Remove(StepGridMsgID.ClickGrid,onClickGrid);
            _playManager.Messenger.Remove(StepGridMsgID.GridDestroy,onGridDestroy);
            _playManager.Messenger.Remove(StepGridMsgID.Exit,onClickBack);
            _playManager.Messenger.Remove(StepGridMsgID.Restart,onClickRestart);
            _playManager.Messenger.Remove(StepGridMsgID.Stop,onGameStop);
            _moduleInput.Dispose();
            _moduleOutput.Dispose();
            base.Dispose();
        }

        private void onGameStop(object obj)
        {
            _frameIndex=0;
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

            return data;
        }

        public static bool GridIsTarget(int index,GridListData gridListData,int gridGroupWidth)
        {
            int line = Index2Line(index,gridGroupWidth);
            int x_i = index%gridGroupWidth;
            int val = gridListData.GridValues[line%gridListData.GridValues.Length];
            return val==x_i;
        }

        public static int Index2Line(int index,int gridGroupWidth)
        {
            return index/gridGroupWidth;
        }

        public static int Index2PlaceID(int index,int gridGroupWidth,int playerCount)
        {
            int line = Index2Line(index,gridGroupWidth);
            return line%playerCount;
        }

        private void onClickGrid(object obj)
        {
            PB_ClickGrid clickGrid = obj as PB_ClickGrid;
            int placeID = Index2PlaceID(clickGrid.Index,_playManager.StepGridConfig.GridGroupWidth,_playManager.GameStart.Players.Count);
            // Debug.LogWarning(placeID);
            if(!GridIsTarget(clickGrid.Index,GridListData,_playManager.StepGridConfig.GridGroupWidth) || clickGrid.PlaceIndex!=placeID)
            {
                // _playManager.Messenger.Broadcast(StepGridMsgID.Stop,placeID);
            }
            _clickGrids.Add(clickGrid.Index);
        }

        private void onGridDestroy(object obj)
        {
            int index = (int)obj;
            int line = index/_playManager.StepGridConfig.GridGroupWidth;
            int placeID = line%_playManager.GameStart.Players.Count;
            if(GridIsTarget(index,GridListData,4) && !_clickGrids.Contains(index))
            {
                // _playManager.Messenger.Broadcast(StepGridMsgID.Stop,placeID);
            }
            _clickGrids.Remove(index);
        }

        private void onClickRestart(object obj)
        {
            for (int i = 0; i < GameReadys.Length; i++)
            {
                GameReadys[i]=false;
            }
        }

        private void onGameReadyReceive(byte[] buffer)
        {
            ushort appID=0,length=0;
            uint cmd=0;
            long gameplayID=0;
            if(buffer.Length>=NetworkUtility.PACK_HEAD_LENGTH && NetworkUtility.Decode(buffer,ref appID,ref length,ref gameplayID,ref cmd))
            {
                if(appID==NetworkUtility.APP_ID && length==buffer.Length && gameplayID==_playManager.GamePlayID)
                {
                    // Debug.LogWarning("cmd "+cmd);
                    if(cmd==NetworkUtility.GAMEREADY_CMD)
                    {
                        PB_GameReady ready = PB_GameReady.Parser.ParseFrom(buffer,NetworkUtility.PACK_HEAD_LENGTH,length-NetworkUtility.PACK_HEAD_LENGTH);
                        lock(_gameReadyLock)
                        {
                            GameReadys[ready.PlaceIndex]=true;

                            if(!Array.Exists<bool>(GameReadys,b=>{return !b;}))
                            {
                                allPlayerReady().Enqueue();
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator allPlayerReady()
        {
            _playManager.Messenger.Broadcast(StepGridMsgID.Start,null);
            _playManager.FrameSyncSystem.OnEnable();
            yield break;
        }

        void Update()
        {
            while(_frameIndex<_playManager.FrameSyncSystem.LogicFrames.Count && _playManager.FrameSyncSystem.LogicFrames[_frameIndex].Complete)
            {
                
                Debug.Log("Complete " +_frameIndex);
                var frames = _playManager.FrameSyncSystem.LogicFrames[_frameIndex].Frames;
                for (int i = 0; i < frames.Length; i++)
                {
                    var frame = frames[i];
                    for (int j = 0; j < frame.Cmds.Count; j++)
                    {
                        switch ((StepGridMsgID)frame.Cmds[j])
                        {
                            case StepGridMsgID.ClickGrid:
                            {
                                var msg = PB_ClickGrid.Parser.ParseFrom(frame.MsgDatas[j]);
                                Debug.Log("click "+msg.PlaceIndex);
                                _playManager.Messenger.Broadcast(StepGridMsgID.ClickGrid,msg);
                            }
                            break;
                        }
                    }
                }
                _frameIndex++;
            }
        }

    }

    public class GridListData
    {
        public byte[] GridValues;
    }
}
