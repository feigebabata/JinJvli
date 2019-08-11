using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System;

namespace JinJvli
{
    public class FileSender
    {
        public static class Config
        {
            /// <summary>
            /// 每次传输数量
            /// </summary>
            public const int SEND_LENGTH=1024*8;
        }
        public int Port
        {
            get;
            private set;
        }
        public long Size
        {
            get;
            private set;
        }
        TcpListener m_self;
        FileStream m_fileStream;
        object m_file_lock = new object();
        string m_filePath;

        int m_recvCount;
        
        public FileSender(string _filePath)
        {
            m_filePath = _filePath;
            Size = new FileInfo(_filePath).Length;
            Port = NetworkManager.Config.FILE_TRANSPORT;
            m_self = new TcpListener(NetworkManager.GetLocalIP(),Port);
            m_self.Start();
            waitConnectAsync();
        }

        async void waitConnectAsync()
        {
            TcpClient recvClient=null;
            try
            {
                recvClient = await m_self.AcceptTcpClientAsync();
            }
            catch (Exception _ex)
            {
                Debug.LogError($"[FileSender.waitConnectAsync] 等待连接失败\n{_ex}");
            }
            if(recvClient != null)
            {
                m_recvCount++;
                await Task.Run(() => { sendFile(recvClient); });
            }
            waitConnectAsync();
        }

        void sendFile(TcpClient _recvClient)
        {
            if(m_fileStream== null)
            {
                m_fileStream = new FileStream(m_filePath,FileMode.Open);
            }
            var _recvStream = _recvClient.GetStream();
            long sendIndex=0;//文件头指针位置 每次read后文件头指针会自动后移
            int sendLength=0;
            byte[] sendBuffer = new byte[Config.SEND_LENGTH];
            while(sendIndex<Size)
            {
                if(_recvClient.Connected)
                {
                    lock (m_file_lock)
                    {
                        m_fileStream.Seek(sendIndex,SeekOrigin.Begin);//有可能同时给多个用户传输同一个文件 文件头指针需重新设置
                        sendLength = m_fileStream.Read(sendBuffer,0,sendBuffer.Length);
                        sendIndex+=sendLength;
                    }

                    try
                    {
                        _recvStream.Write(sendBuffer,0,sendLength);
                    }
                    catch(Exception _ex)
                    {
                        sendErr($"[FileSender.sendFile] 传输失败\n{_ex}");
                        break;
                    }
                }
                else
                {
                    sendErr($"[FileSender.sendFile] 链接断开");
                    break;
                }
            }
            _recvStream.Close();
            _recvStream.Dispose();
            _recvClient.Close();
            _recvClient.Dispose();
            lock (m_file_lock)
            {
                m_recvCount--;
            }
            if(m_recvCount==0)
            {

            }
        }

        void sendErr(string _msg)
        {
            Debug.LogError(_msg);
        }

    }
}