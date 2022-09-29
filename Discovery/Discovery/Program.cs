using System;
using System.IO;
using System.Threading;
using Discovery.Common;
using Discovery.Listener;
using Discovery.Router;
using Discovery.Storage;

namespace Discovery
{
    internal static class Program
    {
        private const string ConfigurationPath = "../../Resources/configuration.json";
		
        public static void Main(string[] args)
        {
            var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;
            var configurationAbsolutePath = Path.Combine(
                directoryAbsolutePath, ConfigurationPath);
			
            var configurator = new Configurator(configurationAbsolutePath);
            var configurationData = configurator.Load();
            
            var storage = new RouteStorage() as IStorage;
            
            var registratorRouter = new RegistratorRouter(storage) as IRouter;
            
            var registratorListener = new AsyncListener(
                configurationData.RegistratorPrefixes, 
                registratorRouter) as IAsyncListener;
            registratorListener.Schedule();

            var discoveryRouter = new DiscoveryRouter(storage);
            
            var discoveryListener = new AsyncListener(
                configurationData.DiscoveryPrefixes, 
                discoveryRouter) as IAsyncListener;
            discoveryListener.Schedule();
            
            Thread.Sleep(Timeout.InfiniteTimeSpan);          
        }
    }
}