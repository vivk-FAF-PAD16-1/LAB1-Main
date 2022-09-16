using System;
using System.Net.Http;

namespace Weather.Service
{
    public class Discovery
    {
        private readonly HttpClient _client;
        private readonly string _uri;
        
        public Discovery(string registerUriPath)
        {
            _client = new HttpClient();
            _uri = registerUriPath;
        }

        public async void Register()
        {
            
            // localhost:40404/ => POST with json in body: endpoint (current_weather) and destinationUri (localhost:8000/current_weather)
            try	
            {
                HttpResponseMessage response = await _client.GetAsync(_uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);

                Console.WriteLine(responseBody);
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
    }
}