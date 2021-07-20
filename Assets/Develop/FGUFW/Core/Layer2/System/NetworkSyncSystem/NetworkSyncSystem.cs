using System;
using System.Collections.Generic;
using System.Threading;
using  System.Threading.Tasks;
using Google.Protobuf;
using UnityEngine;

namespace FGUFW.Core
{
    public class NetworkSyncSystem : INetworkSyncSystem
    {
        public class SendUnit
        {
            public long GamePlayID;
            public uint Cmd;
            public Google.Protobuf.IMessage Msg;
        }

        private IMessenger<uint,ByteString> _messenger = new Messenger<uint,ByteString>();

        private bool _enable;
        private Queue<SendUnit> _sendDataQueue = new Queue<SendUnit>();
        private object _sendDataLock = new object();
        private Queue<PB_Frame> _reveiveDataQueue = new Queue<PB_Frame>();
        private object _receiveDataLock = new object();
        private int _playerID=1;
        private long _gameplayID;
        private int _frameIndex=-1;
        private const uint FRAME_CMD = 11;

        public IMessenger<uint, ByteString> Messenger => _messenger;

        // private byte[] sendBuffer = new byte[1024];

        public void OnInit(params object[] datas)
        {
            _gameplayID = (long)datas[0];
            _playerID = (int)datas[1];
            AndroidBehaviour.I.LockAcquire();
            MonoBehaviourEvent.I.UpdateListener+=Update;
            UdpBroadcastUtility.Init();
            UdpBroadcastUtility.OnReceive += onReceive;
            
            loopCheck();
        }

        public void OnRelease()
        {
            OnDisable();
            MonoBehaviourEvent.I.UpdateListener-=Update;
            UdpBroadcastUtility.OnReceive -= onReceive;
            UdpBroadcastUtility.Release();
            AndroidBehaviour.I.LockAcquire();
            Millisecond=0;
            printTimeDic.Clear();
        }

        public void OnDisable()
        {
            _enable = false;
        }

        public void OnEnable()
        {
            _enable = true;
        }

        private async void loopCheck()
        {
            // Debug.Log("1");
            while (true)
            {
            // Debug.Log(_enable);
                if(_enable && Application.isPlaying)
                {
            // Debug.Log("2");
                    sendFrameData();
                }
                // Thread.Sleep(1000*1/30);
                await Task.Delay(1000*1/60);
            }
        }

        void sendFrameData()
        {
            _frameIndex++;
            int f_idx = _frameIndex;
            PB_Frame frame = new PB_Frame();
            frame.PlayerID = _playerID; 
            frame.Index = f_idx;

            lock(_sendDataLock)
            {
                while(_sendDataQueue.Count>0)
                {
                    SendUnit sendUnit = _sendDataQueue.Dequeue();
                    frame.Cmds.Add(sendUnit.Cmd);
                    frame.MsgDatas.Add(sendUnit.Msg.ToByteString());
                }
            }
            byte[] msgBuffer = frame.ToByteArray();

            var sendBuffer = NetworkUtility.Encode(NetworkUtility.APP_ID,_gameplayID,FRAME_CMD,msgBuffer);
            printTimeDic.Add(f_idx,DateTime.Now.UnixMilliseconds());
            for (int i = 0; i < NetworkUtility.BROADCAST_COUNT; i++)
            {
                UdpBroadcastUtility.Send(sendBuffer);
            }
        }

        public void SendMsg(uint cmd,long gameplayID,object msg)
        {
            lock(_sendDataLock)
            {
                _sendDataQueue.Enqueue(new SendUnit()
                {
                    Msg=(Google.Protobuf.IMessage)msg,
                    Cmd=cmd,
                    GamePlayID=gameplayID,
                });
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
                        // Debug.Log(frame.Index);
                        Millisecond = DateTime.Now.UnixMilliseconds()-printTimeDic[frame.Index];
                        lock(_receiveDataLock)
                        {
                            _reveiveDataQueue.Enqueue(frame);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                throw;
            }
        }

        private void Update()
        {
            if(_enable)
            {
                lock(_receiveDataLock)
                {
                    while (_reveiveDataQueue.Count>0)
                    {
                        var unit = _reveiveDataQueue.Dequeue();
                        broadCaseMsg(unit);
                    }
                }
            }
        }

        void broadCaseMsg(PB_Frame frame)
        {
            for (int i = 0; i < frame.Cmds.Count; i++)
            {
                Messenger.Broadcast(frame.Cmds[i],frame.MsgDatas[i]);
            }
            // Messenger.Broadcast(frame.Cmds,frame.MsgDatas);
        }

        static Dictionary<int,long> printTimeDic = new Dictionary<int, long>();
        public static long Millisecond;


    }
}