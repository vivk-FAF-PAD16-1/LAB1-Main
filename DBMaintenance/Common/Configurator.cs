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
        if (IsValidJson(jsonContent) == false)
        {
            throw new Exception("Configuration file is not in JSON  format.");
        }
            
        var configurationData = JsonSerializer.Deserialize<ConfigurationData>(jsonContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return configurationData;
    }
    
    private static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput)) { return false;}
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (JsonReaderException jex)
            {
                //Exception in parsing json
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}