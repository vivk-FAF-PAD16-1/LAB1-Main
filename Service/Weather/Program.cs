using Weather.TCPServer;

namespace Weather
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var ip = "127.0.0.1";
            var port = 13000;
            
            var tcpServer = new TcpListener();
            tcpServer.Initialize(ip, port);

        }
    }
}