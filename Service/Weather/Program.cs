using System.Threading;
using Weather.Common;
using Weather.Server.Listener;
using Weather.Server.Router;
using Weather.Service;

namespace Weather
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var status = new Status();
            var weatherRouter = new WeatherRouter(status) as IRouter;

            var prefixes = new[] { "http://localhost:8000/" };
            var weatherListener = new AsyncListener(prefixes, weatherRouter) as IAsyncListener;

            weatherListener.Schedule();

            var discovery = new Discovery("http://localhost:40404");

            var endpointDatas = new EndpointData[]
            {
                new EndpointData
                {
                    Endpoint = "current_weather",
                    DestinationUri = "http://localhost:8000/current_weather"
                },
                new EndpointData
                {
                    Endpoint = "status",
                    DestinationUri = "http://localhost:8000/status"
                },
            };

            foreach (var endpointData in endpointDatas)
            {
                discovery.Register(endpointData);
            }
            
            Thread.Sleep(Timeout.InfiniteTimeSpan);
        }
    }
}