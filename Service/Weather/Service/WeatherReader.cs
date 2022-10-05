using MySqlConnector;

namespace Weather.Service
{
    public class WeatherReader
    {
        private MySqlConnection _connection;
        
        public WeatherReader(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);
           
        }
        
        public string GetCurrentWeather(string city)
        {
            var finalData = "";
            // connect to cache and check if there is a current weather
            
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
                
                
            }
            _connection.Close();
            return finalData;
        }
    }
}