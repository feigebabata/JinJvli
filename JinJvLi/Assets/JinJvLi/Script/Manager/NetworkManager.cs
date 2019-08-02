using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Net.NetworkInformation;

namespace JinJvli
{
    public class NetworkManager : IManager
    {
        public static class Config
        {
            public const string BROADCASR_IP = "192.168.1.255";
            public const int BROADCASR_PORT = 30001;
            public const int UDP_CLIENT_PORT=30002;

            /// <summary>
            /// 文件传输接口 多个文件传输时累加
            /// </summary>
            public const int FILE_TRANSPORT=31000;
            public const int PACK_MAX_LENGTH=1024;
        }
        
        Dictionary<Type,IClient> m_clients = new Dictionary<Type, IClient>();

        UdpClient m_sendBroadcastClient,m_receveBroadcastClient;

        IPEndPoint m_broadcastIPEnd;

        int m_receveBroadcastID=-1;

        public void Init()
        {
            m_sendBroadcastClient = new UdpClient(Config.UDP_CLIENT_PORT);
            m_receveBroadcastClient = new UdpClient(Config.BROADCASR_PORT);
            m_broadcastIPEnd = new IPEndPoint(IPAddress.Parse(Config.BROADCASR_IP),Config.BROADCASR_PORT);
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

        public async void SendBroadcast(ISendData _sendData)
        {
            byte[] data = _sendData.Pack();
            try
            {
                var length = await m_sendBroadcastClient.SendAsync(data, data.Length,m_broadcastIPEnd);
                Debug.Log("send "+length);
            }
            catch(Exception _ex)
            {
                Debug.LogError($"[NetworkManager.SendBroadcast]{_ex.Message}");
            }
        }

        async void receveBroadcast()
        {
            byte[] data = null;
            int receveID = m_receveBroadcastID;
            while(receveID==m_receveBroadcastID)
            {
                if(data!=null)
                {
                    Debug.Log("receve "+data.Length);
                }

                try
                {
                    var result = await m_receveBroadcastClient.ReceiveAsync();
                    data = result.Buffer;
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
            receveBroadcast();
            // Task.Run(receveBroadcast);
        }

        public void StopReceveBroadcast()
        {
            m_receveBroadcastID = (int)Time.time;
            m_receveBroadcastClient.Close();
        }

        /// <summary>
        /// 端口是否被占用
        /// </summary>
        /// <param name="_port"></param>
        /// <returns></returns>
        public static bool IsPortOccuped(int _port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            for(int i= 0;i<tcpConnInfoArray.Length;i++)
            {
                if (tcpConnInfoArray[i].LocalEndPoint.Port == _port)
                {
                    return false;
                }
            }
            return true;
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