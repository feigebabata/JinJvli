using JinJvLi.Lobby;
using Google.Protobuf;
using System;
using UnityEngine;

namespace JinJvli
{
    public class GameServer : IServer
    {
        public static class Config
        {
            public const float BROADCASR_TIME=1.5f;
        }

        BroadcastGameData m_gameData;
        NetworkManager m_networkManager;
        Coroutine m_broadcastGame;

        public void Start(PB_GameRoom _gameRoom,NetCmd _cmd)
        {
            m_networkManager = Main.Manager<NetworkManager>();
            _gameRoom.Version = Version();
            m_gameData = new BroadcastGameData(_gameRoom,_cmd);
            m_broadcastGame = Coroutines.Inst.LoopRun(Config.BROADCASR_TIME,-1,broadcastGame);
        }

        public float Version()
        {
            return 1.0f;
        }

        public void Close()
        {
            Coroutines.Inst.Stop(m_broadcastGame);
        }

        void broadcastGame()
        {
            m_networkManager.SendBroadcast(m_gameData);
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