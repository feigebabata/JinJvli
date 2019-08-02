using System.Net.Sockets;

namespace JinJvli
{
    public class FileSender
    {
        Socket m_selfSocket;
        public int Port
        {
            get;
            private set;
        }
        public FileSender(string FilePath)
        {

        }
    }
}