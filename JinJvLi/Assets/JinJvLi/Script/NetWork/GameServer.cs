using JinJvLi.Lobby;
using Google.Protobuf;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace JinJvli
{
    public class GameServer : IServer
    {
        public static class Config
        {
            public const float BROADCASR_TIME=1.5f;
        }
        List<IPEndPoint> m_clients = new List<IPEndPoint>();
        UdpClient m_server;
        bool m_isRun;


        public void Start(int _port)
        {
            m_server = new UdpClient(_port);
            recvCmdAsync();
            // m_networkManager = Main.Manager<NetworkManager>();
            // _gameRoom.Version = Version();
            // m_gameData = new BroadcastGameData(_gameRoom,_cmd);
            // m_broadcastGame = Coroutines.Inst.LoopRun(Config.BROADCASR_TIME,-1,broadcastGame);
        }

        public float Version()
        {
            return 1.0f;
        }

        public void Close()
        {
            // Coroutines.Inst.Stop(m_broadcastGame);
            m_server.Close();
            m_server.Dispose();
        }

        // void broadcastGame()
        // {
        //     m_networkManager.SendBroadcast(m_gameData);
        // }

        async void recvCmdAsync()
        {
            UdpReceiveResult serverRecvResult;
            while(m_isRun)
            {
                try
                {
                    serverRecvResult = await m_server.ReceiveAsync();
                    sendCmd(serverRecvResult.Buffer);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[GameServer.recvCmdAsync]接受消息异常!{ex}");
                }
            }
        }

        async void sendCmd(byte[] _data)
        {
            for (int i = 0; i < m_clients.Count; i++)
            {
                await m_server.SendAsync(_data,_data.Length,m_clients[i]);
            }
        }
    }

    public struct BroadcastGameData : ISendData
    {
        byte[] sendData;
        public BroadcastGameData(PB_GameRoom _gameRoom,NetCmd _cmd)
        {
            byte[] data = _gameRoom.ToByteArray();
            UInt32 cmd = (UInt32)_cmd;
            sendData = new byte[data.Length+NetworkManager.Config.NET_CMD_LENGTH];
            Array.Copy(BitConverter.GetBytes(cmd),sendData,NetworkManager.Config.NET_CMD_LENGTH);
            Array.Copy(data,0,sendData,NetworkManager.Config.NET_CMD_LENGTH,data.Length);
        }

        public byte[] Pack()
        {
            return sendData;
        }
    }
}