using System.Net.Sockets;
using System.Net;

namespace JinJvli
{
    public class ChatClient : IClient
    {
        public void Close()
        {
            
        }

        public void Connect(string _ip, int _port)
        {
            
        }

        public byte[] Pack()
        {
            return null;
        }

        public T Unpack<T>(byte[] _data)
        {
            return default(T);
        }
    }
}