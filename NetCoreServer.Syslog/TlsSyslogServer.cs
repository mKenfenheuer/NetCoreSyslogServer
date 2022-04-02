
using System.Net;

namespace NetCoreServer.Syslog
{

    public class SyslogTlsSession 
        : NetCoreServer.SslSession
    {
        protected ISyslogMessageHandler m_messageHandler;
        public SyslogTlsSession(NetCoreServer.SslServer server, ISyslogMessageHandler handler) 
            : base(server) 
        {
            if (handler == null)
                throw new ArgumentNullException("Handler cannot be null.");
            this.m_messageHandler = handler;
        }
        protected override void OnConnected()
        {
            System.Console.WriteLine($"Syslog SSL session with Id {Id} connected!");
        }
        protected override void OnHandshaked()
        {
            System.Console.WriteLine($"Syslog SSL session with Id {Id} handshaked!");
        }
        protected override void OnDisconnected()
        {
            System.Console.WriteLine($"Syslog SSL session with Id {Id} disconnected!");
        }
        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            this.m_messageHandler.OnReceived(this.Socket.RemoteEndPoint, buffer, offset, size);
        }
        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            this.m_messageHandler.OnError(error);
        }
    }
    public class TlsSyslogServer 
        : NetCoreServer.SslServer, IControllableServer
    {
        protected ISyslogMessageHandler m_messageHandler;

        public TlsSyslogServer(
              NetCoreServer.SslContext context
            , System.Net.IPAddress address
            , int port
            , ISyslogMessageHandler handler
        ) 
            : base(context, address, port) 
        {
            this.m_messageHandler = handler;
        }
        protected override NetCoreServer.SslSession CreateSession() 
        { 
            return new SyslogTlsSession(this, this.m_messageHandler);
        }
        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            System.Console.WriteLine($"Syslog SSL server caught an error with code {error}");
        }
        public static bool AllowAnything(
              object sender
            , System.Security.Cryptography.X509Certificates.X509Certificate certificate
            , System.Security.Cryptography.X509Certificates.X509Chain chain
            , System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        Task IControllableServer.Start()
        {
            base.Start();
            return Task.CompletedTask;
        }

        Task IControllableServer.Stop()
        {
            base.Start();
            return Task.CompletedTask;
        }
    }
}
