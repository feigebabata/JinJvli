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
            public int PlayerID;
            public uint Cmd;
            public Google.Protobuf.IMessage Msg;
        }
        private bool _enable;
        private Queue<SendUnit> _sendDataQueue = new Queue<SendUnit>();
        private object _sendDataLock = new object();
        private Queue<PB_MsgData> _reveiveDataQueue = new Queue<PB_MsgData>();
        private object _receiveDataLock = new object();
        private int _playerID;

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
            PB_MsgData sendData = new PB_MsgData();
            sendData.Cmd = 0;
            sendData.PlayerID = _playerID; 
            lock(_sendDataLock)
            {
                while(_sendDataQueue.Count>0)
                {
                    SendUnit sendUnit = _sendDataQueue.Dequeue();
                    if(sendUnit.Msg!=null)
                    {
                        sendData.MsgData = sendUnit.Msg.ToByteString();
                    }
                    sendData.Cmd = sendUnit.Cmd;
                    byte[] data = sendData.ToByteArray();
                    UdpBroadcastUtility.Send(data);
                }
            }
        }

        public void SendMsg(object msg,uint cmd)
        {
            lock(_sendDataLock)
            {
                _sendDataQueue.Enqueue(new SendUnit(){Msg=(Google.Protobuf.IMessage)msg,Cmd=cmd});
            }
        }

        private void onReceive(byte[] obj)
        {
            PB_MsgData receiveData=null;
            try
            {
                receiveData = PB_MsgData.Parser.ParseFrom(obj);
                lock(_receiveDataLock)
                {
                    _reveiveDataQueue.Enqueue(receiveData);
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
                        broadCaseMsg(_reveiveDataQueue.Dequeue());
                    }
                }
            }
        }

        void broadCaseMsg(PB_MsgData msgData)
        {
            UnityEngine.Debug.Log("cmd "+msgData.Cmd);
        }

    }
}