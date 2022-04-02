using System.Net;
using System.Net.Sockets;

namespace NetCoreServer.Syslog
{
    public interface ISyslogMessageHandler
    {
        void OnError(SocketError error);
        void OnReceived(EndPoint? remoteEndPoint, byte[] buffer, long offset, long size);
    }
}