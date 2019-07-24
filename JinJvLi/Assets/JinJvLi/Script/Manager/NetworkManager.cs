using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace JinJvli
{
    public class NetworkManager : IManager
    {
        public static class Config
        {
            public const string BroadcastIP = "192.168.1.255";
            public const int BroadcastPort = 30001;
            public const int UdpClientPort=30002;
        }
        
        Dictionary<Type,IClient> m_clients = new Dictionary<Type, IClient>();

        UdpClient m_sendBroadcastClient,m_receveBroadcastClient;

        IPEndPoint m_broadcastIPEnd;

        int m_receveBroadcastID=-1;

        public void Init()
        {
            m_sendBroadcastClient = new UdpClient(Config.UdpClientPort);
            m_receveBroadcastClient = new UdpClient(Config.BroadcastPort);
            m_broadcastIPEnd = new IPEndPoint(IPAddress.Parse(Config.BroadcastIP),Config.BroadcastPort);
            StartReceveBroadcast();
        }

        public void Clear()
        {
            StopReceveBroadcast();
            m_sendBroadcastClient.Close();
            m_sendBroadcastClient.Dispose();
            m_receveBroadcastClient.Dispose();
        }

        public void Update()
        {
            
        }

        public T Clients<T>() where T : IClient
        {
            Type clientType = typeof(T);
            if(!m_clients.ContainsKey(clientType))
            {
                IClient client = Activator.CreateInstance(clientType) as IClient;
                m_clients[clientType]=client;
            }
            return (T)m_clients[clientType];
        }

        public void SendBroadcast(ISendData _sendData)
        {
            byte[] data = _sendData.Pack();
            try
            {
                m_sendBroadcastClient.SendAsync(data, data.Length,m_broadcastIPEnd);
            }
            catch(Exception _ex)
            {
                Debug.LogError($"[NetworkManager.SendBroadcast]{_ex.Message}");
            }
        }

        void receveBroadcast()
        {
            byte[] data = null;
            int receveID = m_receveBroadcastID;
            while(receveID==m_receveBroadcastID)
            {
                if(data!=null)
                {

                }

                try
                {
                    data = m_receveBroadcastClient.Receive(ref m_broadcastIPEnd);
                }
                catch (Exception _ex)
                {
                    data = null;
                    Debug.LogError($"[NetworkManager.receveBroadcast]{_ex.Message}");
                }
            }
        }
        public void StartReceveBroadcast()
        {
            m_receveBroadcastID = (int)Time.time;
            Task.Run(receveBroadcast);
        }

        public void StopReceveBroadcast()
        {
            m_receveBroadcastID = (int)Time.time;
            m_receveBroadcastClient.Close();
        }
        
    }
    
    public interface ISendData
    {
        byte[] Pack();
    }

    public interface IReceveData
    {
        T Unpack<T>(byte[] _data);
    }

    public interface IClient
    {
        void Connect(string _ip,int _port);
        void Close();
    }

    public interface IServer
    {
        void Start();
        void Stop();
    }
}