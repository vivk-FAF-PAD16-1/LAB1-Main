using DBMaintenance.Common;

namespace DBMaintenance;

internal static class Program
{
    private const string ConfigurationPath = "Resources\\configuration.json";

    public static void Main(string[] args)
    {
        var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;

        var configurationAbsolutePath = Path.Combine(
            directoryAbsolutePath, "..\\..\\..", ConfigurationPath);

        var configurator = new Configurator(configurationAbsolutePath);
        var configurationData = configurator.Load();
        
        
    }
}
