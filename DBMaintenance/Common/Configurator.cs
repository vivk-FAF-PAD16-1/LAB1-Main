using System.Text.Json;
using DBMaintenance.Common.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DBMaintenance.Common;

public class Configurator
{
    private readonly string _path;
        
    public Configurator(string path)
    {
        _path = path;
    }

    public ConfigurationData Load()
    {
        if (File.Exists(_path) == false)
        {
            throw new Exception("Can not load configuration file.");
        }

        var jsonContent = File.ReadAllText(_path);
        if (JsonUtilities.IsValid(jsonContent) == false)
        {
            throw new Exception("Configuration file is not in JSON  format.");
        }
            
        var configurationData = JsonSerializer.Deserialize<ConfigurationData>(jsonContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return configurationData;
    }
}