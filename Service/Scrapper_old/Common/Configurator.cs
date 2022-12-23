using System;
using System.IO;
using System.Text.Json;
using Scrapper.Common.Data;

namespace Scrapper.Common
{
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
    
}