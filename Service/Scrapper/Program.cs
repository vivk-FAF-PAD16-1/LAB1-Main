using System;
using System.IO;
using Scrapper.Common;

namespace Scrapper
{
    internal class Program
    {
        private const string ConfigurationPath = "../../Resourses/configuration.json";
		
        public static void Main(string[] args)
        {
            var directoryAbsolutePath = AppDomain.CurrentDomain.BaseDirectory;
            var configurationAbsolutePath = Path.Combine(
                directoryAbsolutePath, ConfigurationPath);
			
            var configurator = new Configurator(configurationAbsolutePath);
            var configurationData = configurator.Load();

            var sqlConnectionString = $"Server={configurationData.MySqlAddress};User ID={configurationData.UserId};Password={configurationData.UserPwd};" +
                                      $"Database={configurationData.DB}";
            
            
            
        }
    }
}