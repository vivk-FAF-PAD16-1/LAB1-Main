using System;
using System.IO;
using System.Threading;
using Gateway.Common;
using Gateway.Listener;
using Gateway.Router;

namespace Gateway
{
	internal static class Program
	{
		private const string ConfigurationPath = "../../Resourses/configuration.json";
		
		public static void Main(string[] args)
		{
			var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;
			var configurationAbsolutePath = Path.Combine(
				directoryAbsolutePath, ConfigurationPath);
			
			var configurator = new Configurator(configurationAbsolutePath);
			var configurationData = configurator.Load();
			
			var gatewayRouter = new GatewayRouter(configurationData.DiscoveryUri);
			
			var gatewayListener = new AsyncListener(
				configurationData.GatewayPrefixes, 
				gatewayRouter) as IAsyncListener;
			gatewayListener.Schedule();

			Thread.Sleep(Timeout.InfiniteTimeSpan);
		}
	}
}