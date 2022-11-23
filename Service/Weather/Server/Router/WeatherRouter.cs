using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Weather.Service;
using Weather.SlotStorage;

namespace Weather.Server.Router
{
    public class WeatherRouter : IRouter
    {
        private readonly WeatherReader _weatherReader;
        private readonly Status _status;

        private static ISlotStorage _slots;
        
        public WeatherRouter(WeatherReader weatherReader, Status status)
        {
            _weatherReader = weatherReader;
            _status = status;

            _slots = new RequestSlotStorage(1);
        }
        
        public async void Route(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (_slots.IsFull())
            {
                TooManyRequests(response);
                return;
            }
            
            _slots.Load();
            await RouteInternal(request, response);
            _slots.Unload();
        }
        
        private async Task RouteInternal(HttpListenerRequest request, HttpListenerResponse response)
        {
            Console.WriteLine("Got request");
            _status.OnCallReceived();
            
            if (request.HttpMethod != "GET")
            {
                NotFound(response);
                return;
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
                    var text = "";
                    
                    try
                    {
                        text = await TimeoutAfter(
                            Task.Run(() => _weatherReader.GetCurrentWeather(request.Url.Segments[2].TrimEnd('/'))),
                            new TimeSpan(0, 0, 10));
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine(e.Message);
                        TimeOut(response);
                        return;
                    }
                    
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
        
        private void TimeOut(HttpListenerResponse response, string notFoundMessage = "408 Request Timeout!")
        {
            var buffer = Encoding.UTF8.GetBytes(notFoundMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = (int) HttpStatusCode.RequestTimeout;
            
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

        public static async Task<TResult> TimeoutAfter<TResult>(Task<TResult> task, TimeSpan timeout)
        {
            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task; // Very important in order to propagate exceptions
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }
        
        private void TooManyRequests(HttpListenerResponse response, string notFoundMessage = "429 Too Many Requests!")
        {
            var buffer = Encoding.UTF8.GetBytes(notFoundMessage);
            
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = buffer.Length;
            response.StatusCode = 429; // 429 = TooManyRequests;
            
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            _status.OnCallSent();
        }
    }
}