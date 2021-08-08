
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FGUFW.Core
{
    public class SyncClient : IDisposable
    {
        private TcpClient _client;
        public Action<string> OnConnect;
        public Func<byte[],int,int> OnReceive;

        public SyncClient(string ip,int port)
        {
            _client = new TcpClient();

            connectServer(_client,ip,port);
        }

        public void Dispose()
        {
            try
            {
                _client.Close();
            }
            catch {}
            _client=null;
        }

        private async void connectServer(TcpClient client, string ip, int port)
        {
            string err = null;
            try
            {
                await client.ConnectAsync(ip,port);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                err = ex.ToString();
            }
            OnConnect?.Invoke(err);
            receive(client);
        }

        private async void receive(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024*2];
            int length = 0,index=0;
            while (client!=null && client.Connected)
            {
                try
                {
                    length = await stream.ReadAsync(buffer,index,buffer.Length-index);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(ex);
                }
                index+=length;
                if(length>0 && OnReceive!=null)
                {
                    int count = OnReceive(buffer,index);
                    index-=count;
                }
            }
        }


        public async void Send(byte[] buffer, int index=0)
        {
            var stream = _client.GetStream();
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