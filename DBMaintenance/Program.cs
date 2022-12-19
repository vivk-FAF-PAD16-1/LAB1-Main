using DBMaintenance.Common;
using DBMaintenance.Listener;
using DBMaintenance.Router;
using DBMaintenance.Service;

namespace DBMaintenance;

internal static class Program
{
    private const string ConfigurationPath = "resources/configuration.json";

    public static void Main(string[] args)
    {
        var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;

        var configurationAbsolutePath = Path.Combine(
            directoryAbsolutePath, ConfigurationPath);

         var configurator = new Configurator(configurationAbsolutePath);
         var configurationData = configurator.Load();
        
         var maintenanceService = new MaintenanceService(configurationData.MySqlAddresses);
        
         var maintenanceRouter = new MaintenanceRouter(maintenanceService);
             
         var maintenanceListener = new AsyncListener(
             configurationData.MaintenancePrefixes, 
             maintenanceRouter) as IAsyncListener;
         maintenanceListener.Schedule();
        
        Thread.Sleep(Timeout.InfiniteTimeSpan);
    }
}
