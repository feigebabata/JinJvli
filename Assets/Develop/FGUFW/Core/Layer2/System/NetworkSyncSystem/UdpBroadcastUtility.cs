using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FGUFW.Core
{
    public static class UdpBroadcastUtility
    {
        public const int BROADCAST_PORT = 31410;
        public const int LOCAL_PORT = 31411;

        private static UdpClient sendClient,receiveClient;
        private static IPEndPoint broadcastIEP;
        public static Action<byte[]> OnReceive;
        private static byte[] receiveBuffer;

        public static void Init()
        {
            if(broadcastIEP!=null)
            {
                return;
            }

            broadcastIEP = new IPEndPoint(IPAddress.Broadcast,BROADCAST_PORT);
            sendClient = new UdpClient(LOCAL_PORT);
            receiveClient = new UdpClient(BROADCAST_PORT);
            receive();
        }

        /// <summary>
        /// 发送数据 [ appid 2 | length 2 | gameplayid 8 |cmd 4| msgdata ]
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async void Send(byte[] data)
        {
            try
            {
                await sendClient.SendAsync(data,data.Length,broadcastIEP);
                // Debug.Log("发送成功");
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        static async void receive()
        {
            try
            {
                receiveBuffer = null;
                var result = await receiveClient.ReceiveAsync(); 
                receiveBuffer = result.Buffer;
            }
            catch// (System.Exception ex)
            {
                // Debug.LogError(ex);
            }

            if(receiveBuffer!=null)
            {
                OnReceive?.Invoke(receiveBuffer);
            }

            if(broadcastIEP!=null)
            {
                receive();
            }
        }

        public static void Release()
        {
            if(broadcastIEP==null)
            {
                return;
            }

            broadcastIEP = null;
            try
            {
                sendClient.Close();
                sendClient.Dispose();
                receiveClient.Close();
                receiveClient.Dispose();
            }
            catch (System.Exception)
            {
                throw;
            }
            sendClient=null;
            receiveClient=null;
        }
        
        
    }
}
