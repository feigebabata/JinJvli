using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using JinJvLi.Lobby;
using UnityEngine;

namespace JinJvli
{
    public class FileRecver
    {
        public static class Config
        {
            /// <summary>
            /// 每次传输数量
            /// </summary>
            public const int SEND_LENGTH=1024*8;
        }
        PB_FileTransfer m_transfer;
        TcpClient m_self;
        string m_filePath;
        public double Result
        {
            get;
            private set;
        }

        public FileRecver(PB_FileTransfer _transfer,string _dirPath)
        {
            m_transfer = _transfer;
            m_filePath = Path.Combine(_dirPath,m_transfer.FileName);
            m_self = new TcpClient();
            Task.Run(receve);
        }

        void receve()
        {
            if(File.Exists(m_filePath))
            {
                File.Delete(m_filePath);
            }
            var fileStream = File.Create(m_filePath);
            try
            {
                m_self.Connect(m_transfer.Address.IP,m_transfer.Address.Port);
            }
            catch (System.Exception _ex)
            {
                err($"[FileRecver.receve] 连接服务器失败\n{_ex}");
            }
            var sendSteam = m_self.GetStream();
            long recvSize=0;
            int readLength;
            byte[] recvBuffer = new byte[Config.SEND_LENGTH];
            while(recvSize<m_transfer.FileSize)
            {
                try
                {
                    readLength = sendSteam.Read(recvBuffer,0,recvBuffer.Length);
                    recvSize+=readLength;
                }
                catch (System.Exception _ex)
                {
                    err($"[FileRecver.receve] 接受数据失败\n{_ex}");
                    break;
                }
                fileStream.Write(recvBuffer,0,readLength);
                Result = (double)recvSize/m_transfer.FileSize;
            }
            fileStream.Close();
            fileStream.Dispose();
            sendSteam.Close();
            sendSteam.Dispose();
        }

        void err(string _msg)
        {
            clear();
            Result=-1;
            Debug.LogError(_msg);
        }

        void clear()
        {
            m_self.Close();
            m_self.Dispose();
        }

    }
}