using System.Threading;
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
            
            Thread.Sleep(Timeout.InfiniteTimeSpan);
        }
    }
}