using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MySqlConnector;

namespace Weather.Service
{
    public class WeatherReader
    {
        private readonly HttpClient _client;
        
        private MySqlConnection _connection;
        private string _cacheAddressUri;
        private string _gatewayAddressUri;
        
        public WeatherReader(string sqlConnectionString, string cacheAddressUri, string gatewayAddressUri)
        {
            _connection = new MySqlConnection(sqlConnectionString);
            _cacheAddressUri = cacheAddressUri;
            _gatewayAddressUri = gatewayAddressUri;
            _client = new HttpClient();
        }
        
        public string GetCurrentWeather(string city)
        {
            var finalData = "";
            var cacheKey = "CurrentWeather." + city;
            // connect to cache and check if there is a current weather
            var (ok, cacheData) = GetDataFromCache(cacheKey).Result;

            if (ok)
                return cacheData;
            
            // connect to database if cache has nothing there
            _connection.Open();
        
            var command = new MySqlCommand($"SELECT cw.weather FROM city " +
                                           $"INNER JOIN current_weather cw on city.id = cw.city_id " +
                                           $"WHERE city_name = '{city}';", _connection);
            var reader = command.ExecuteReader();
            string data = "";
            bool isFound = false;
            while (reader.Read())
            {
                data = reader.GetString(0);
                isFound = true;
                break;
            }

            if (isFound)
            {
                finalData = data;
                // connect to cache to pass it the new data it doesnt have
                AddDataToCache(cacheKey, finalData);
            }
            else
            {
                var (ok2, scrapedData) = GetDataFromScrapper($"scrape_current_weather/{city}").Result;
                if (ok2)
                {
                    finalData = scrapedData;
                    AddDataToCache(cacheKey, finalData);
                }
            }
            _connection.Close();
            return finalData;
        }

        private async Task<(bool, string)> GetDataFromScrapper(string endpoint)
        {
            var uri = _gatewayAddressUri + endpoint;
            try
            {
                var response = await _client.GetAsync(uri);
                return (true, response.Content.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
            
            
            
            return (false, "");
        }

        private async void AddDataToCache(string key, string data)
        {
            try
            {
                var jsonData = $"{{\"{key}\": {data}}}";
                Console.WriteLine(jsonData);
                
                var token = "weather_token";
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _client.PostAsync(_cacheAddressUri + "cache/post", content);
                response.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        private async Task<(bool, string)> GetDataFromCache(string key)
        {
            try
            {
                var token = "weather_token";
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                string responseBody = await _client.GetStringAsync(_cacheAddressUri + "cache/get/" + key);

                Console.WriteLine(responseBody);
                
                if (responseBody != "null\n")
                    return (true, responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
                return (false, "");
            }
            
            return (false, "");
        }
    }
}