
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FGUFW.Core
{
    public class SyncServer : IDisposable
    {
        public const int PORT_MIN=33000;
        
        public const int PORT_MAX=33100;
        private int _playerMaxCount;
        private TcpListener _server;

        public IPEndPoint LocalIPEndPoint => _server?.LocalEndpoint as IPEndPoint;
        private Dictionary<string,TcpClient> _clients = new Dictionary<string, TcpClient>();
        private bool _acceptEnd=false;
        public object Data;

        public SyncServer(int playerMaxCount)
        {
            _playerMaxCount = playerMaxCount;
            int port = PORT_MIN;
            var ip = NetworkUtility.GetIP();
            while (port<PORT_MAX)
            {
                try
                {
                    _server = new TcpListener(ip,port);
                    break;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
                port++;
            }
            _server.Start();
            acceptClient(_server);
        }

        public void Dispose()
        {
            _server.Stop();
            _server=null;
            foreach (var kv in _clients)
            {
                try
                {
                    kv.Value.Close();
                }
                catch {}
            }
            _clients.Clear();
        }

        private async void acceptClient(TcpListener tcpListener)
        {
            while (!_acceptEnd && _server!=null && _clients.Count<_playerMaxCount)
            {
                try
                {
                    var client = await tcpListener.AcceptTcpClientAsync();
                    string key = client.Client.RemoteEndPoint.ToString();
                    Debug.Log("接受链接"+key);
                    _clients.Add(key,client);
                    receive(client);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
            }
        }

        private async void receive(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024*2];
            int length = 0;
            while (client.Connected)
            {
                try
                {
                    length = await stream.ReadAsync(buffer,0,buffer.Length);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
                if(length>0)
                {
                    onReceive(buffer,length);
                }
            }
        }

        private void onReceive(byte[] buffer, int length)
        {
            foreach (var kv in _clients)
            {
                send(kv.Value,buffer,length);
            }
        }

        private async void send(TcpClient client, byte[] buffer, int length)
        {
            var stream = client.GetStream();
            try
            {
                await stream.WriteAsync(buffer,0,buffer.Length);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}