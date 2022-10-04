using System;
using System.Web;
using MySqlConnector;
using Scrapper.Common.Data;

namespace Scrapper.Service
{
    public class WeatherScrapper
    {
        private MySqlConnection _connection;
        private string _apiKey;
        
        public WeatherScrapper(string sqlConnectionString, string openWeatherApiKey)
        {
            _connection = new MySqlConnection(sqlConnectionString);
            _apiKey = openWeatherApiKey;
        }

        public (int, string) FindCurrentWeather(string city)
        {
            var data = "";

            var cityData = FindCityData(city);

            if (cityData is null)
                return (0, "");
            
            Console.WriteLine($"{cityData.Name}, {cityData.Lat}, {cityData.Lon}");

            //var query = HttpUtility.ParseQueryString(string.Empty);
            

            return (1, data);
        }

        private CityData FindCityData(string cityName)
        {
            var command = new MySqlCommand($"SELECT city_name, lat, lon FROM city " +
                                           $"WHERE city_name = '{cityName}';", _connection);
            var reader = command.ExecuteReader();

            var cityData = new CityData();
            
            bool isFound = false;
            while (reader.Read())
            {
                cityData.Name = reader.GetString(0);
                cityData.Lat = reader.GetFloat(1);
                cityData.Lon = reader.GetFloat(2);
                isFound = true;
                break;
            }

            if (!isFound)
            {
                cityData = ScrapeCityData(cityName);
            }
            
            
            
            return cityData;
        }

        
        
        private CityData ScrapeCityData(string cityName)
        {

            return null;
        }
        
        
    }
}