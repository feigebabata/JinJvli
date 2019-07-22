using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;
using System.Threading.Tasks;

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
        
        UdpClient m_udpSendClient,m_udpReceveClient;

        IPEndPoint m_broadcastIPEnd;

        int m_receveBroadcastID=-1;

        public void Init()
        {
            m_udpSendClient = new UdpClient(Config.UdpClientPort);
            m_udpReceveClient = new UdpClient(Config.BroadcastPort);
            m_broadcastIPEnd = new IPEndPoint(IPAddress.Parse(Config.BroadcastIP),Config.BroadcastPort);
            StartReceveBroadcast();
        }

        public void Clear()
        {
            m_udpReceveClient.Close();
            m_udpSendClient.Close();
        }

        public void Update()
        {
            
        }

        public void SendBroadcast(ISendData _sendData)
        {
            byte[] data = _sendData.Pack();
            try
            {
                m_udpSendClient.SendAsync(data, data.Length,m_broadcastIPEnd);
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
                    data = m_udpReceveClient.Receive(ref m_broadcastIPEnd);
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
        }
        
    }
}