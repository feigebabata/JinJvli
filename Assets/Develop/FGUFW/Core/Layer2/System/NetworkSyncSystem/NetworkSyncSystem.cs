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
            public int PlayerID;
            public uint Cmd;
            public Google.Protobuf.IMessage Msg;
        }
        public class ReceiveUnit
        {
            public ushort GamePlayID;
            public PB_MsgData MsgData;
        }

        public IMessenger<ushort,PB_MsgData> Messenger = new Messenger<ushort,PB_MsgData>();

        private bool _enable;
        private Queue<SendUnit> _sendDataQueue = new Queue<SendUnit>();
        private object _sendDataLock = new object();
        private Queue<ReceiveUnit> _reveiveDataQueue = new Queue<ReceiveUnit>();
        private object _receiveDataLock = new object();
        private int _playerID;
        // private byte[] sendBuffer = new byte[1024];

        public void OnInit()
        {
            AndroidBehaviour.I.LockAcquire();
            MonoBehaviourEvent.I.UpdateListener+=Update;
            UdpBroadcastUtility.Init();
            UdpBroadcastUtility.OnReceive += onReceive;
            var _loopTask = new Thread(loopCheck);
            _loopTask.Start();
        }

        public void OnRelease()
        {
            OnDisable();
            MonoBehaviourEvent.I.UpdateListener-=Update;
            UdpBroadcastUtility.OnReceive -= onReceive;
            UdpBroadcastUtility.Release();
            AndroidBehaviour.I.LockAcquire();
        }

        public void OnDisable()
        {
            _enable = false;
        }

        public void OnEnable()
        {
            _enable = true;
        }

        private void loopCheck()
        {
            // Debug.Log("1");
            while (true)
            {
            // Debug.Log(_enable);
                if(_enable)
                {
            // Debug.Log("2");
                    sendMsg();
                }
                Thread.Sleep(1000*1/30);
            }
        }

        void sendMsg()
        {
            PB_MsgData msg = new PB_MsgData();
            msg.Cmd = 0;
            msg.PlayerID = _playerID; 
            lock(_sendDataLock)
            {
                while(_sendDataQueue.Count>0)
                {
                    SendUnit sendUnit = _sendDataQueue.Dequeue();
                    if(sendUnit.Msg!=null)
                    {
                        msg.MsgData = sendUnit.Msg.ToByteString();
                    }
                    msg.Cmd = sendUnit.Cmd;
                    byte[] msgBuffer = msg.ToByteArray();
                    ushort bufferLength = (ushort)(msgBuffer.Length+NetworkConfig.PACK_HEAD_LENGTH);
                    byte[] sendBuffer = new byte[bufferLength];
                    byte[] appIDBuffer = BitConverter.GetBytes(NetworkConfig.APP_ID);
                    byte[] gpIDBuffer = BitConverter.GetBytes(sendUnit.GamePlayID);
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
                    for (int i = 0; i < NetworkConfig.BROADCAST_COUNT; i++)
                    {
                        UdpBroadcastUtility.Send(sendBuffer);
                    }
                }
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
            PB_MsgData msgData=null;
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
                index += NetworkConfig.PACK_GAMEPLAY_LENGTH;
                
                msgData = PB_MsgData.Parser.ParseFrom(buffer,index,buffer.Length-index);

                lock(_receiveDataLock)
                {
                    _reveiveDataQueue.Enqueue(new ReceiveUnit()
                    {
                        GamePlayID=gameplayID,
                        MsgData=msgData,
                    });
                }
            }
            catch (System.Exception)
            {
                
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
                        broadCaseMsg(unit.GamePlayID,unit.MsgData);
                    }
                }
            }
        }

        void broadCaseMsg(ushort gameplayID,PB_MsgData msgData)
        {
            UnityEngine.Debug.Log("cmd "+msgData.Cmd);
            Messenger.Broadcast(gameplayID,msgData);
        }

    }
}