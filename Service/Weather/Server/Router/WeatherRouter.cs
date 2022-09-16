using System;
using System.Net;
using System.Text;
using Weather.Service;

namespace Weather.Server.Router
{
    public class WeatherRouter : IRouter
    {
        private readonly WeatherReader _weatherReader;
        private readonly Status _status;

        public WeatherRouter(Status status)
        {
            _weatherReader = new WeatherReader();
            _status = status;
        }

        public void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            _status.OnCallReceived();
            
            if (request.HttpMethod != "GET")
            {
                NotFound(response);
                return;
            }

            foreach (var seg in request.Url.Segments)
            {
                Console.WriteLine(seg);
            }

            if (request.Url.Segments.Length == 1)
            {
                NotFound(response);
                return;
            }
            
            // do work

            var endpoint = request.Url.Segments[1].TrimEnd('/');
            
            switch (endpoint)
            {
                case "current_weather":
                    _status.OnCallAccepted();
                    var text = _weatherReader.GetCurrentWeather(request.Url.Segments[2].TrimEnd('/'));
                    SendJson(text, response);
                    break;
                case "status":
                    _status.OnCallAccepted();
                    text = _status.GetStatus();
                    SendJson(text, response);
                    break;
                default:
                    NotFound(response);
                    return;
            }
        }
        
        private void NotFound(HttpListenerResponse response, string notFoundMessage = "404 Not Found!")
        {
            var buffer = Encoding.UTF8.GetBytes(notFoundMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = (int) HttpStatusCode.NotFound;;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            _status.OnCallSent();
        }

        private async void SendJson(string json, HttpListenerResponse response)
        {
            var data = Encoding.UTF8.GetBytes(json);

            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;

            // Write out to the response stream (asynchronously), then close it
            await response.OutputStream.WriteAsync(data, 0, data.Length);
            response.Close();
            _status.OnCallSent();
        }
    }
}