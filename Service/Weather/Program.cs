using System;
using System.Threading;
using Weather.Server;
using Weather.Server.Listener;
using Weather.Server.Router;
using Weather.Service;

namespace Weather
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // var ip = "127.0.0.1";
            // var port = 13000;
            //
            // var tcpServer = new TcpListener();
            // tcpServer.Initialize(ip, port);
            
            //HttpServer.Start();

            var status = new Status();
            var weatherRouter = new WeatherRouter(status) as IRouter;

            var prefixes = new[] { "http://localhost:8000/" };
            var weatherListener = new AsyncListener(prefixes, weatherRouter) as IAsyncListener;

            
            
            weatherListener.Schedule();
            
            Thread.Sleep(100000);
            
            weatherListener.Stop();
        }
    }
}