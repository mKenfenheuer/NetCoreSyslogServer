
using System.Net;

namespace NetCoreServer.Syslog
{
    public enum NetworkProtocol
    {
        UDP,
        TCP,
        TLS
    }
    public class SyslogServer
    {
        public int Port { get; private set; }
        public NetworkProtocol Protocol { get; private set; }
        public IPAddress IPAddress { get; private set; }
        private ISyslogMessageHandler _handler;
        private SslContext? _sslContext;

        private IControllableServer _controllableServer;

        public SyslogServer(int port, NetworkProtocol protocol, IPAddress iPAddress, ISyslogMessageHandler handler)
        {
            Port = port;
            Protocol = protocol;
            IPAddress = iPAddress;
            switch (Protocol)
            {
                case NetworkProtocol.TCP:
                    _controllableServer = new TcpSyslogServer(IPAddress, Port, handler);
                    break;
                case NetworkProtocol.TLS:
                    if (_sslContext == null)
                        throw new InvalidOperationException("A SSL Context is required to use SSL. Either the provided context was NULL or you used the wrong constructor. Use the constructor SyslogServer(int port, IPAddress iPAddress, ISyslogMessageHandler handler, SslContext sslContext) constructor instead.");
                    _controllableServer = new TlsSyslogServer(_sslContext, IPAddress, Port, handler);
                    break;
                default:
                    _controllableServer = new UdpSyslogServer(IPAddress, Port, handler);
                    break;
            }
            _handler = handler;
        }

        public SyslogServer(int port, IPAddress iPAddress, ISyslogMessageHandler handler, SslContext sslContext) : this(port, NetworkProtocol.TLS, iPAddress, handler)
        {
            _sslContext = sslContext;
        }

        public Task StartAsync() => _controllableServer.Start();
        public void Start()
        {
            _controllableServer.Start().Wait();
        }

        public Task StopAsync() => _controllableServer.Stop();
        public void Stop()
        {
            _controllableServer.Stop().Wait();
        }
    }
}
