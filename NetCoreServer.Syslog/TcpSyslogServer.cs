using System.Net;

namespace NetCoreServer.Syslog
{
    public class SyslogTcpSession
        : NetCoreServer.TcpSession
    {
        protected ISyslogMessageHandler m_messageHandler;
        public SyslogTcpSession(NetCoreServer.TcpServer server, ISyslogMessageHandler handler) 
            : base(server)
        {
            if (handler == null)
                throw new ArgumentNullException("Handler cannot be null.");
            this.m_messageHandler = handler;
        }
        protected override void OnConnected()
        {
            System.Console.WriteLine($"Syslog TCP session with Id {Id} connected!");
            string message = "Hello from Syslog TCP session ! Please send a message or '!' to disconnect the client!";
            SendAsync(message);
        }
        protected override void OnDisconnected()
        {
            System.Console.WriteLine($"Syslog TCP session with Id {Id} disconnected!");
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
    public class TcpSyslogServer 
        : NetCoreServer.TcpServer, IControllableServer
    {
        protected ISyslogMessageHandler m_messageHandler;
        public TcpSyslogServer(
              System.Net.IPAddress address
            , int port
            , ISyslogMessageHandler handler 
        ) 
            : base(address, port) 
        {
            this.m_messageHandler = handler;
        }

        protected override NetCoreServer.TcpSession CreateSession() 
        { 
            return new SyslogTcpSession(this, this.m_messageHandler);
        }
        protected override void OnError(System.Net.Sockets.SocketError error)
        {
            this.m_messageHandler.OnError(error);
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
