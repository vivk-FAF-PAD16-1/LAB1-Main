using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using MySqlConnector;
using Newtonsoft.Json.Linq;
using Scrapper.Common;
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

            var cityData = FindCityData(city).Result;

            if (cityData is null)
                return (0, "");
            
            Console.WriteLine($"{cityData.Name}, {cityData.Lat}, {cityData.Lon}");

            //var query = HttpUtility.ParseQueryString(string.Empty);
            

            return (1, data);
        }

        private async Task<CityData> FindCityData(string cityName)
        {
            _connection.Open();
            
            var command = new MySqlCommand($"SELECT city_name, lat, lon FROM city " +
                                           $"WHERE city_name = '{cityName}';", _connection);
            var reader = await command.ExecuteReaderAsync();

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
            
            await _connection.CloseAsync();

            if (!isFound)
            {
                cityData = await ScrapeCityData(cityName);

                if (cityData != null)
                {
                    AddCityDataToBd(cityData);
                }
            }

            return cityData;
        }

        
        
        private async Task<CityData> ScrapeCityData(string cityName)
        {
            var builder = new UriBuilder("http://api.openweathermap.org/geo/1.0/direct");
            builder.Port = -1;

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["q"] = cityName;
            query["limit"] = "1";
            query["appid"] = _apiKey;

            builder.Query = query.ToString();
            string url = builder.ToString();

            var client = new HttpClient();

            try	
            {
                string responseBody = await client.GetStringAsync(url);
                Console.WriteLine(responseBody);
                
                var jArray = JArray.Parse(responseBody);

                var cityData = new CityData();
                cityData.Name = cityName.Capitalize();
                cityData.Lat = jArray[0]["lat"].Value<float>();
                cityData.Lon = jArray[0]["lon"].Value<float>();

                return cityData;
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
            
            
            return null;
        }

        private void AddCityDataToBd(CityData cityData)
        {
            _connection.Open();
            var cmd = new MySqlCommand($"INSERT INTO city (city_name, lat, lon)" +
                                           $"VALUES ('{cityData.Name}', @lat, @lon);", _connection);


            cmd.Parameters.Add("lat", MySqlDbType.Float).Value = cityData.Lat; //_acc.Character.ShapeMix;
            cmd.Parameters.Add("lon", MySqlDbType.Float).Value = cityData.Lon; //_acc.Character.SkinMix;
            cmd.ExecuteNonQuery();

            _connection.Close();
        }


    }
}