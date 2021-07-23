
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

namespace FGUFW.Core
{
    public class FrameSyncSystem : IFrameSyncSystem
    {
        public const int FRAME_DELAY = 1000/30;

        private Action<LogicFrame> _logicFrameUpdate;
        private int _playerCount;

        public Action<LogicFrame> OnLogicFrameUpdate { get => _logicFrameUpdate;set => _logicFrameUpdate=value;}
        public List<LogicFrame> LogicFrames{get;private set;}
        private object logicFramesLock = new object();
        private PB_Frame _sendBuffer;
        private object _sendBufferLock = new object();
        private bool _enable=false;

        private long _gameplayID;
        private const uint FRAME_CMD = 12;

        public void OnInit(params object[] datas)
        {
            _playerCount = (int)datas[0];
            int placeID = (int)datas[1];
            _gameplayID = (long)datas[2];
            LogicFrames = new List<LogicFrame>(1024);
            _sendBuffer = new PB_Frame()
            {
                Index = 0,
                PlaceIndex = placeID,
            };
            UdpBroadcastUtility.Init();
            UdpBroadcastUtility.OnReceive += onReceive;

            sendFrame();
        }

        public void OnRelease()
        {
            
        }

        public void OnDisable()
        {
            _enable = false;
        }

        public void OnEnable()
        {
            _enable = true;
        }

        public void PushCmd(uint cmd,IMessage message)
        {
            lock (_sendBufferLock)
            {
                _sendBuffer.Cmds.Add(cmd);
                _sendBuffer.MsgDatas.Add(message.ToByteString());
            }
        }

        private async void sendFrame()
        {
            while (true)
            {
                if(_enable && Application.isPlaying)
                {
                    

                    byte[] msgBuffer = null;
                    lock(_sendBufferLock)
                    {
                        msgBuffer = _sendBuffer.ToByteArray();
                        _sendBuffer.Cmds.Clear();
                        _sendBuffer.MsgDatas.Clear();
                        _sendBuffer.Index++;
                    }
                    var sendBuffer = NetworkUtility.Encode(NetworkUtility.APP_ID,_gameplayID,FRAME_CMD,msgBuffer);
                    for (int i = 0; i < NetworkUtility.BROADCAST_COUNT; i++)
                    {
                        UdpBroadcastUtility.Send(sendBuffer);
                    }
                }
                await Task.Delay(FRAME_DELAY);
            }
        }

        private void onReceive(byte[] buffer)
        {
            PB_Frame frame = null;
            try
            {
                ushort appID=0,length=0;
                uint cmd=0;
                long gameplayID=0;
                if(buffer.Length>=NetworkUtility.PACK_HEAD_LENGTH && NetworkUtility.Decode(buffer,ref appID,ref length,ref gameplayID,ref cmd))
                {
                    // Debug.Log("cmd "+cmd);
                    if(cmd==FRAME_CMD)
                    {
                        frame = PB_Frame.Parser.ParseFrom(buffer,NetworkUtility.PACK_HEAD_LENGTH,buffer.Length-NetworkUtility.PACK_HEAD_LENGTH);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }
            if(frame!=null)
            {
                lock (logicFramesLock)
                {
                    int frameIndex = frame.Index;
                    int placeID = frame.PlaceIndex;
                    while(frameIndex >= LogicFrames.Count)
                    {
                        LogicFrames.Add(new LogicFrame()
                        {
                            Frames = new PB_Frame[_playerCount],
                        });
                    }
                    var logicFrame = LogicFrames[frameIndex];
                    logicFrame.Frames[placeID]=frame;
                    int frame_place_count = 0;
                    for (int i = 0; i < _playerCount; i++)
                    {
                        if(logicFrame.Frames[placeID]!=null)
                        {
                            frame_place_count++;
                        }
                    }
                    if(frame_place_count==_playerCount)
                    {
                        OnLogicFrameUpdate?.Invoke(logicFrame);
                    }
                }
            }
        }
        
    }

}