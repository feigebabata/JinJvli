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

        public class ClientUser
        {
            public IPEndPoint IP;
            public PB_UserInfo Info;
            public ClientUser(PB_UserInfo _info)
            {
                Info = _info;
                IP = new IPEndPoint(IPAddress.Parse(Info.Address.IP),Info.Address.Port);
            }
        }
        List<ClientUser> m_clients = new List<ClientUser>();
        UdpClient m_server;
        bool m_isRun;
        NetworkManager m_netMng;
        PB_GameRoom _gameRoom;
        BroadcastGameData m_gameData;
        Coroutine m_broadcastGame;

        public int Port{get;private set;}

        public GameServer()
        {
            m_netMng = Main.Manager<NetworkManager>();
            Port = NetworkManager.Config.GAME_SERVER_PORT;
            m_server = new UdpClient(Port);    
        }

        public void Start(PB_GameRoom _gameRoom)
        {
            m_isRun = true;
            
            recvCmdAsync();
            
            _gameRoom.Version = Version();
            _gameRoom.Address = new PB_IPAddress(){IP=_gameRoom.Host.Address.IP,Port=Port};
            m_gameData = new BroadcastGameData(_gameRoom);
            m_broadcastGame = Coroutines.Inst.LoopRun(Config.BROADCASR_TIME,-1,broadcastGame);
        }

        public float Version()
        {
            return 1.0f;
        }

        public void Close()
        {
            m_broadcastGame.Stop();
            m_server.Close();
            m_server.Dispose();
        }

        void broadcastGame()
        {
            m_netMng.SendBroadcast(m_gameData);
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
                    joinGame(_data);
                break;
                case NetCmd.ExitGame:
                    exitGame(_data);
                break;
                default:
                    sendCmd(_data);
                break;
            }
        }

        async void redundancySend(byte[] _data)
        {
            for (int i = 0; i < m_clients.Count; i++)
            {
                for (int j = 0; j < NetworkManager.Config.SEND_REDUNDANCY; j++)
                {
                    await m_server.SendAsync(_data,_data.Length,m_clients[i].IP);
                }
            }
        }

        async void sendCmd(byte[] _data)
        {
            for (int i = 0; i < m_clients.Count; i++)
            {
                await m_server.SendAsync(_data,_data.Length,m_clients[i].IP);
            }
        }

        void joinGame(byte[] _data)
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
            if(m_clients.Find((_user)=>{return _user.Info.Address.Port==user.Address.Port && _user.Info.Address.IP==user.Address.IP;})==null)
            {
                user.GameID = m_clients.Count;
                m_clients.Add(new ClientUser(user));
                Array.Copy(user.ToByteArray(),0,_data,NetworkManager.Config.NET_CMD_LENGTH,_data.Length-NetworkManager.Config.NET_CMD_LENGTH);
                redundancySend(_data);
            }
        }

        void exitGame(byte[] _data)
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
            var clientUser = m_clients.Find((_user)=>{return _user.Info.Address.Port==user.Address.Port && _user.Info.Address.IP==user.Address.IP;});
            if(clientUser!=null)
            {
                m_clients.Remove(clientUser);
                Array.Copy(BitConverter.GetBytes((UInt16)NetCmd.ExitGame),_data,NetworkManager.Config.NET_CMD_LENGTH);
                redundancySend(_data);
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