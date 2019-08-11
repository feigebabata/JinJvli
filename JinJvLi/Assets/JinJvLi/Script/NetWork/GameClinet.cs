using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace JinJvli
{
    public class GameClinet : IClient
    {
        public class GameCmd : Broadcaster.IMsg
        {
            public NetCmd Cmd;
            public byte[] Buffer;
        }
        UdpClient m_client;
        IPEndPoint m_serverIP;
        int m_receveID=-1;
        object m_cmdQueueLock = new object();
        Queue<GameCmd> m_cmdQueue = new Queue<GameCmd>();
        UdpReceiveResult m_cmdRecvResult;

        public int Port{get;private set;}

        public GameClinet()
        {
            Port = NetworkManager.Config.GAME_CLIENT_PORT;
            m_client = new UdpClient(Port);        
        }

        public void Connect(string _ip, int _port)
        {
            m_serverIP = new IPEndPoint(IPAddress.Parse(_ip),_port);
            m_receveID =  (int)Time.time;
            receveCmd();
        }

        public void Close()
        {
            m_receveID =  (int)Time.time;
            m_client?.Close();
            m_client?.Dispose();
        }

        public void Update()
        {
            lock(m_cmdQueueLock)
            {
                while(m_cmdQueue.Count>0)
                {
                    var pack = m_cmdQueue.Dequeue();
                    Broadcaster.Broadcast(pack);
                }

            }
        }

        public async void SendCmd(byte[] _data)
        {
            try
            {
                var length = await m_client.SendAsync(_data, _data.Length,m_serverIP);
            }
            catch(Exception _ex)
            {
                Debug.LogError($"[NetworkManager.SendBroadcast]{_ex.Message}");
            }
        }

        public void RedundancySend(byte[] _data)
        {
            for (int j = 0; j < NetworkManager.Config.SEND_REDUNDANCY; j++)
            {
                SendCmd(_data);
            }
        }

        async void receveCmd()
        {
            byte[] data = null;
            int receveID = m_receveID;
            while(receveID==m_receveID)
            {
                if(data!=null)
                {
                    GameCmd gameCmd=new GameCmd();
                    gameCmd.Cmd = (NetCmd)BitConverter.ToUInt16(data,0);
                    gameCmd.Buffer = new byte[data.Length-NetworkManager.Config.NET_CMD_LENGTH];
                    Array.Copy(data,NetworkManager.Config.NET_CMD_LENGTH,gameCmd.Buffer,0,gameCmd.Buffer.Length);
                    lock (m_cmdQueueLock)
                    {
                        m_cmdQueue.Enqueue(gameCmd);
                    }
                }

                try
                {
                    m_cmdRecvResult = await m_client.ReceiveAsync();
                    data = m_cmdRecvResult.Buffer;
                }
                catch (Exception _ex)
                {
                    data = null;
                    Debug.LogError($"[GameClinet.receveBroadcast]{_ex.Message}");
                    return;
                }
            }
        }
    }
}