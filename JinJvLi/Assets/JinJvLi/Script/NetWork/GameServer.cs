using JinJvLi.Lobby;
using Google.Protobuf;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using JinJvLi;

namespace JinJvli
{
    public enum GameState
    {
        Ready,Run,Pause,Over
    }
    public class GameServer : IServer
    {
        public static class Config
        {
            public const float BROADCASR_TIME=1.5f;
        }
        List<IPEndPoint> m_clients = new List<IPEndPoint>();
        UdpClient m_server;
        bool m_isRun;
        NetworkManager m_networkManager;
        PB_GameRoom _gameRoom;
        BroadcastGameData m_gameData;
        Coroutine m_broadcastGame;

        public void Start(int _port,PB_GameRoom _gameRoom)
        {
            m_networkManager = Main.Manager<NetworkManager>();
            
            var ip = NetworkManager.GetLocalIP();
            m_server = new UdpClient(new IPEndPoint(ip,_port));

            recvCmdAsync();
            
            _gameRoom.Version = Version();
            _gameRoom.Address = new PB_IPAddress(){IP=ip.ToString(),Port=_port};
            m_gameData = new BroadcastGameData(_gameRoom);
            m_broadcastGame = Coroutines.Inst.LoopRun(Config.BROADCASR_TIME,-1,broadcastGame);
        }

        public float Version()
        {
            return 1.0f;
        }

        public void Close()
        {
            Coroutines.Inst.Stop(m_broadcastGame);
            m_server.Close();
            m_server.Dispose();
        }

        void broadcastGame()
        {
            m_networkManager.SendBroadcast(m_gameData);
        }

        async void recvCmdAsync()
        {
            UdpReceiveResult serverRecvResult;
            while(m_isRun)
            {
                try
                {
                    serverRecvResult = await m_server.ReceiveAsync();
                    switchCmd(serverRecvResult.Buffer);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[GameServer.recvCmdAsync]接受消息异常!{ex}");
                }
            }
        }

        void switchCmd(byte[] _data)
        {
            var cmd = (NetCmd)BitConverter.ToUInt16(_data,0);
            switch (cmd)
            {
                case NetCmd.JoinGame:
                {
                    PB_UserInfo user;
                    try
                    {
                        user = PB_UserInfo.Parser.ParseFrom(_data,NetworkManager.Config.NET_CMD_LENGTH,_data.Length-NetworkManager.Config.NET_CMD_LENGTH);
                    }
                    catch (System.Exception _ex)
                    {
                        Debug.LogError($"[GameServer.switchCmd]PB_UserInfo解析异常!{_ex}");
                        return;
                    }
                    if(m_clients.Find((_ip)=>{return _ip.Port==user.Address.Port && _ip.Address.ToString()==user.Address.IP;})==null)
                    {
                        m_clients.Add(new IPEndPoint(IPAddress.Parse(user.Address.IP),user.Address.Port));
                    }
                }
                break;
                default:
                sendCmd(_data);
                break;
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
        public BroadcastGameData(PB_GameRoom _gameRoom)
        {
            byte[] data = _gameRoom.ToByteArray();
            UInt16 cmd = (UInt16)NetCmd.GameRoom;
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