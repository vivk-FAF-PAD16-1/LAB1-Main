using System.Net;
using System.Text;
using Scrapper.Service;

namespace Scrapper.Router;

public class ScrapperRouter : IRouter
{
    private readonly Status _status;
    private readonly WeatherScrapper _weatherScrapper;

    private static SemaphoreSlim _pool;

    public ScrapperRouter(Status status, WeatherScrapper weatherScrapper)
    {
        _status = status;
        _weatherScrapper = weatherScrapper;

        _pool = new SemaphoreSlim(1, 4);
    }

    public async void Route(HttpListenerRequest request, HttpListenerResponse response)
    {
        await _pool.WaitAsync();
        await RouteInternal(request, response);
        _pool.Release();
    }

    private async Task RouteInternal(HttpListenerRequest request, HttpListenerResponse response)
    {
        _status.OnCallReceived();

        if (request.HttpMethod != "GET")
        {
            NotFound(response, " not get ?");
            return;
        }

        // foreach (var seg in request.Url.Segments)
        // {
        //     Console.WriteLine(seg);
        // }

        if (request.Url.Segments.Length == 1)
        {
            NotFound(response, "???");
            return;
        }

        // do work

        var endpoint = request.Url.Segments[1].TrimEnd('/');

        switch (endpoint)
        {
            case "current_weather":
                _status.OnCallAccepted();

                if (request.Url.Segments.Length != 3)
                {
                    NotFound(response, "bad am segm");
                    return;
                }

                var text = "";
                var ok = false;

                try
                {
                    (ok, text) = await TimeoutAfter(
                        Task.Run(() => _weatherScrapper.FindCurrentWeather(request.Url.Segments[2].TrimEnd('/'))),
                        new TimeSpan(0, 0, 30));
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine(e.Message);
                    TimeOut(response);

                }

                if (ok)
                    SendJson(text, response);
                else
                    NotFound(response, "404 No Such City!");

                break;
            case "status":
                _status.OnCallAccepted();
                text = _status.GetStatus();
                SendJson(text, response);
                break;
            default:
                NotFound(response, " def !");
                return;
        }
    }

    private void NotFound(HttpListenerResponse response, string notFoundMessage = "404 Not Found!")
    {
        var buffer = Encoding.UTF8.GetBytes(notFoundMessage);

        response.ContentEncoding = Encoding.UTF8;
        response.ContentLength64 = buffer.Length;
        response.StatusCode = (int)HttpStatusCode.NotFound;
        ;

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
        response.StatusCode = (int)HttpStatusCode.RequestTimeout;
        ;

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
}