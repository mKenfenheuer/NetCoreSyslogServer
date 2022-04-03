using NetCoreServer.Syslog;
using System.Net;
using System.Net.Sockets;

namespace Samples.Syslog.Server
{
    public static class Program
    {
        [STAThread]
        public static void Main (string[] args)
        {
            SyslogMessageHandler handler = new SyslogMessageHandler();
            SyslogServer server = new SyslogServer(514, NetworkProtocol.UDP, IPAddress.Any, handler);
            server.Start();
            Console.ReadLine();
        }
    }

    public class SyslogMessageHandler : ISyslogMessageHandler
    {
        public void OnError(SocketError error)
        {
        }

        public void OnReceived(EndPoint? remoteEndPoint, byte[] buffer, long offset, long size)
        {
            byte[] data = new byte[size];
            Array.Copy(buffer, offset, data, 0, size);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(data));
        }
    }
}
