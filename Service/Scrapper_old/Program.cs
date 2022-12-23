﻿using System;
using System.IO;
using System.Threading;
using Scrapper.Common;
using Scrapper.Common.Data;
using Scrapper.Server.Listener;
using Scrapper.Server.Router;
using Scrapper.Service;

namespace Scrapper
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

            var sqlConnectionString = $"Server={configurationData.MySqlAddress};User ID={configurationData.UserId};Password={configurationData.UserPwd};" +
                                      $"Database={configurationData.DB}";

            var weatherScrapper = new WeatherScrapper(sqlConnectionString, configurationData.OpenWeatherApiKey);

            var status = new Status();
            
            var scrapperRouter = new ScrapperRouter(status, weatherScrapper);
			
            var scrapperListener = new AsyncListener(
                configurationData.ScrapperPrefixes,
                scrapperRouter) as IAsyncListener;
            scrapperListener.Schedule();

            #region Registration to discovery
            
            var discovery = new Discovery(configurationData.RegistrationUri);

            var endpointsData = new []
            {
                new EndpointData
                {
                    Endpoint = "scrape_current_weather",
                    DestinationUri = "http://localhost:40555/current_weather"
                },
                new EndpointData
                {
                    Endpoint = "scrapper_status",
                    DestinationUri = "http://localhost:40555/status"
                },
            };

            foreach (var endpointData in endpointsData)
            {
                discovery.Register(endpointData);
            }

            #endregion
            
            Thread.Sleep(Timeout.InfiniteTimeSpan);
        }
    }
}