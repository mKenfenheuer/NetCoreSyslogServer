
using System.Net;

namespace NetCoreServer.Syslog
{
    public class UdpSyslogServer 
        : NetCoreServer.UdpServer, IControllableServer
    {
        protected ISyslogMessageHandler m_messageHandler;
        public UdpSyslogServer(System.Net.IPAddress address, int port, ISyslogMessageHandler handler) 
            : base(address, port)
        {
            if (handler == null)
                throw new ArgumentNullException("Handler cannot be null.");
            this.m_messageHandler = handler;
        }
        protected override void OnStarted()
        {
            ReceiveAsync();
        }
        protected override void OnReceived(
              System.Net.EndPoint endpoint
            , byte[] buffer
            , long offset
            , long size)
        {
            this.m_messageHandler.OnReceived(endpoint, buffer, offset, size);
            ReceiveAsync();
        }
        protected override void OnSent(System.Net.EndPoint endpoint, long sent)
        {
            ReceiveAsync();
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
