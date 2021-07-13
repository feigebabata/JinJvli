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
            public ushort GamePlayID;
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
        private ushort _gameplayID;
        private int _frameIndex=-1;

        public IMessenger<uint, ByteString> Messenger => _messenger;

        // private byte[] sendBuffer = new byte[1024];

        public void OnInit(params object[] datas)
        {
            _gameplayID = (ushort)datas[0];
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
                    frame.Cmds=sendUnit.Cmd;
                    frame.MsgDatas=sendUnit.Msg.ToByteString();
                }
            }
            byte[] msgBuffer = frame.ToByteArray();
            ushort bufferLength = (ushort)(msgBuffer.Length+NetworkConfig.PACK_HEAD_LENGTH);
            byte[] sendBuffer = new byte[bufferLength];
            byte[] appIDBuffer = BitConverter.GetBytes(NetworkConfig.APP_ID);
            byte[] gpIDBuffer = BitConverter.GetBytes(_gameplayID);
            byte[] lengthBuffer = BitConverter.GetBytes(bufferLength);

            int index = 0,length=0;

            length = appIDBuffer.Length;
            Array.Copy(appIDBuffer,0,sendBuffer,index,length);
            index+=length;

            length = lengthBuffer.Length;
            Array.Copy(lengthBuffer,0,sendBuffer,index,length);
            index+=length;

            length = gpIDBuffer.Length;
            Array.Copy(gpIDBuffer,0,sendBuffer,index,length);
            index+=length;

            length = msgBuffer.Length;
            Array.Copy(msgBuffer,0,sendBuffer,index,length);
            index+=length;
            printTimeDic.Add(f_idx,DateTime.Now.Millisecond);
            for (int i = 0; i < NetworkConfig.BROADCAST_COUNT; i++)
            {
                UdpBroadcastUtility.Send(sendBuffer);
            }
        }

        public void SendMsg(uint cmd,ushort gameplayID,object msg)
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
                int index=0;

                int appID = BitConverter.ToUInt16(buffer,index);
                index += NetworkConfig.PACK_APPID_LENGTH;

                if(appID!=NetworkConfig.APP_ID)
                {//不是这个应用的消息
                    Debug.LogWarning("不是这个应用的消息");
                    return;
                }

                ushort length = BitConverter.ToUInt16(buffer,index);
                index += NetworkConfig.PACK_LEN_LENGTH;
                if(length!=buffer.Length)
                {//数据包不完整
                    Debug.LogWarning("数据包不完整");
                    return;
                }

                ushort gameplayID = BitConverter.ToUInt16(buffer,index);
                if(gameplayID!=_gameplayID)
                {//数据包不完整
                    // Debug.LogWarning("数据包不完整");
                    return;
                }

                index += NetworkConfig.PACK_GAMEPLAY_LENGTH;
                
                frame = PB_Frame.Parser.ParseFrom(buffer,index,buffer.Length-index);
                
                Millisecond = DateTime.Now.Millisecond-printTimeDic[frame.Index];
                lock(_receiveDataLock)
                {
                    _reveiveDataQueue.Enqueue(frame);
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
            // for (int i = 0; i < frame.Cmds.Count; i++)
            // {
            //     Messenger.Broadcast(frame.Cmds[i],frame.MsgDatas[i]);
            // }
            Messenger.Broadcast(frame.Cmds,frame.MsgDatas);
        }

        static Dictionary<int,int> printTimeDic = new Dictionary<int, int>();
        public static int Millisecond;


    }
}