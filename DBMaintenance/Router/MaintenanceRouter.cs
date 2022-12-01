using System.Net;
using System.Text;
using DBMaintenance.Common;
using DBMaintenance.Service;

namespace DBMaintenance.Router;

public class MaintenanceRouter : IRouter
{
    private readonly MaintenanceService _maintenanceService;
    public MaintenanceRouter(MaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }
    
    public async void Route(HttpListenerRequest request, HttpListenerResponse response)
    {
        await RouteInternal(request, response);
    }

    private async Task RouteInternal(HttpListenerRequest request, HttpListenerResponse response)
    {
        if (request.HttpMethod != "GET")
        {
            NotFound(response, "No status endpoint yet!");
            return;
        }

        // if (request.Url.Segments.Length == 1)
        // {
        //     NotFound(response);
        //     return;
        // }

        Ok(request, response, _maintenanceService.GetCurrentSqlDatabaseAddress());
    }

    #region Useful Methods

    public void Ok(HttpListenerRequest request, HttpListenerResponse response, string message = "Ok!")
    {
        var requestContent = HttpUtilities.ReadRequestBody(request);

        var client = new HttpClient();
        var newRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), message);
        newRequest.Content = new StringContent(requestContent, Encoding.UTF8, request.ContentType);
			
        var newResponse = client.SendAsync(newRequest)
            .GetAwaiter()
            .GetResult();
        var body = HttpUtilities.ReadResponseBody(newResponse);
			
        var buffer = Encoding.UTF8.GetBytes(body);
            
        response.ContentEncoding = Encoding.UTF8;
        response.ContentLength64 = buffer.Length;
        response.StatusCode = (int)newResponse.StatusCode;
            
        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
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
    }

    private void TimeOut(HttpListenerResponse response, string notFoundMessage = "408 Request Timeout!")
    {
        var buffer = Encoding.UTF8.GetBytes(notFoundMessage);

        response.ContentEncoding = Encoding.UTF8;
        response.ContentLength64 = buffer.Length;
        response.StatusCode = (int)HttpStatusCode.RequestTimeout;

        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
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
    }


    #endregion
}