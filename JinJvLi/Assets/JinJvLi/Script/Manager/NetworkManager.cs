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
            public static string Broadcast_IP;
            public const int BROADCASR_PORT = 30001;
            public const int UDP_CLIENT_PORT=30002;

            /// <summary>
            /// 文件传输接口 多个文件传输时累加
            /// </summary>
            public const int FILE_TRANSPORT=31000;
            public const int PACK_MAX_LENGTH=1024;
            public const UInt32 NET_CMD_LENGTH = sizeof(UInt32);
        }

        public struct NetBroadcast: Broadcaster.IMsg
        {
            public NetCmd Cmd;
            public byte[] Buffer;
        }
        
        Dictionary<Type,IClient> m_clients = new Dictionary<Type, IClient>();

        UdpClient m_sendBroadcastClient,m_receveBroadcastClient;

        IPEndPoint m_broadcastIPEnd;

        int m_receveBroadcastID=-1;
        List<IServer> m_servers = new List<IServer>();
        NetBroadcast m_netBroadcast = new NetBroadcast();

        public void Init()
        {
            var ipv4 = GetLocalIP().ToString().Split('.');
            Config.Broadcast_IP = $"{ipv4[0]}.{ipv4[1]}.{ipv4[2]}.255";

            m_sendBroadcastClient = new UdpClient(Config.UDP_CLIENT_PORT);
            m_receveBroadcastClient = new UdpClient(Config.BROADCASR_PORT);
            m_broadcastIPEnd = new IPEndPoint(IPAddress.Parse(Config.Broadcast_IP),Config.BROADCASR_PORT);
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

        public T CreateServer<T>() where T : IServer
        {
            Type clientType = typeof(T);
            IServer server = Activator.CreateInstance(clientType) as IServer;
            m_servers.Add(server);
            return (T)server;
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
                    m_netBroadcast.Cmd = (NetCmd)BitConverter.ToUInt32(data,0);
                    m_netBroadcast.Buffer = new byte[data.Length-Config.NET_CMD_LENGTH];
                    Array.Copy(data,Config.NET_CMD_LENGTH,m_netBroadcast.Buffer,0,m_netBroadcast.Buffer.Length);
                    Broadcaster.Broadcast(m_netBroadcast);
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
        
        public static IPAddress GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i];
                    }
                }
                return null;
            }
            catch (Exception _ex)
            {
                Debug.LogError($"[NetworkManager.GetLocalIP]{_ex.Message}");
                return null;
            }
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
        float Version();
    }
}