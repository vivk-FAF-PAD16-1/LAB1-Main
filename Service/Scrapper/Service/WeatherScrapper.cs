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

        public (bool, string) FindCurrentWeather(string city)
        {
            var weatherData = "";
            

            var (ok, cityData) = FindCityData(city).Result;

            if (!ok)
                return (ok, "");

            weatherData = ScrapeWeatherData(cityData).Result;
            AddWeatherDataToBd(cityData, weatherData);

            return (ok, weatherData);
        }

        private async Task<string> ScrapeWeatherData(CityData cityData)
        {
            var builder = new UriBuilder("https://api.openweathermap.org/data/2.5/weather");
            builder.Port = -1;

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["lat"] = cityData.Lat.ToString();
            query["lon"] = cityData.Lon.ToString();
            query["appid"] = _apiKey;

            builder.Query = query.ToString();
            string url = builder.ToString();

            var client = new HttpClient();

            try	
            {
                string responseBody = await client.GetStringAsync(url);

                // var jArray = JArray.Parse(responseBody);
                //
                // cityData.Name = cityName.Capitalize();
                // cityData.Lat = jArray[0]["lat"].Value<float>();
                // cityData.Lon = jArray[0]["lon"].Value<float>();
                //
                // return cityData;
                
                return responseBody;
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }

            return "";
        }

        private async Task<(bool, CityData)> FindCityData(string cityName)
        {
            _connection.Open();
            
            var command = new MySqlCommand($"SELECT city_name, lat, lon FROM city " +
                                           $"WHERE city_name = '{cityName}';", _connection);
            var reader = await command.ExecuteReaderAsync();

            var cityData = new CityData();
            
            while (reader.Read())
            {
                cityData.Name = reader.GetString(0);
                cityData.Lat = reader.GetFloat(1);
                cityData.Lon = reader.GetFloat(2);
                return (true, cityData);
            }
            
            await _connection.CloseAsync();
            bool ok;

            (ok, cityData) = await ScrapeCityData(cityName);

            if (ok)
            {
                AddCityDataToBd(cityData);
            }

            return (ok, cityData);
        }
        
        private async Task<(bool, CityData)> ScrapeCityData(string cityName)
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
                if (jArray.Count == 0)
                    return (false, null);
                
                var cityData = new CityData();
                cityData.Name = cityName.Capitalize();
                cityData.Lat = jArray[0]["lat"].Value<float>();
                cityData.Lon = jArray[0]["lon"].Value<float>();

                return (true, cityData);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
            
            return (false, null);
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


        private void AddWeatherDataToBd(CityData cityData, string weatherData)
        {
            _connection.Open();
            var cmd = new MySqlCommand($"INSERT INTO current_weather (city_id, weather)" +
                                       $"VALUES (" +
                                       $"(select id from city where city.city_name='{cityData.Name}')," +
                                       $" '{weatherData}');",
                _connection);
            
            cmd.ExecuteNonQuery();
            
            _connection.Close();
        }
    }
}