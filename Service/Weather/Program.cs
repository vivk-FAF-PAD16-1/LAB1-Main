using System;
using System.IO;
using System.Threading;
using Weather.Common;
using Weather.Server.Listener;
using Weather.Server.Router;
using Weather.Service;

namespace Weather
{
    internal class Program
    {
        private const string ConfigurationPath = "Resources/configuration.json";
        
        public static void Main(string[] args)
        {
            var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;

            var configurationAbsolutePath = Path.Combine(
                directoryAbsolutePath, "../..", ConfigurationPath);
			
            var configurator = new Configurator(configurationAbsolutePath);
            var configurationData = configurator.Load();

            var status = new Status();
            
            var sqlConnectionString = $"Server={configurationData.MySqlAddress};User ID={configurationData.UserId};Password={configurationData.UserPwd};" +
                                      $"Database={configurationData.DB}";

            var weatherReader = new WeatherReader(sqlConnectionString, configurationData.CacheAddressUri,
                configurationData.GatewayAddressUri);

            var weatherRouter = new WeatherRouter(weatherReader, status) as IRouter;

            var prefixes = configurationData.WeatherPrefixes;
            var weatherListener = new AsyncListener(prefixes, weatherRouter) as IAsyncListener;

            weatherListener.Schedule();

            var discovery = new Discovery(configurationData.RegistrationUri);

            var endpointsData = new []
            {
                new EndpointData
                {
                    Endpoint = "current_weather",
                    DestinationUri = "http://localhost:40444/current_weather"
                },
                new EndpointData
                {
                    Endpoint = "weather_status",
                    DestinationUri = "http://localhost:40444/status"
                },
            };

            foreach (var endpointData in endpointsData)
            {
                discovery.Register(endpointData);
            }
            
            Thread.Sleep(Timeout.InfiniteTimeSpan);
        }
    }
}