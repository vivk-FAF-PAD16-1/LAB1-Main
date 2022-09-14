using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Weather.Service;

namespace Weather.Server
{
    public class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int requestCount = 0;

        public static WeatherReader WeatherReader = new WeatherReader();


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine(req.ContentType);
                string text;
                using (var reader = new StreamReader(req.InputStream,
                           req.ContentEncoding))
                {
                    text = reader.ReadToEnd();
                }
                
                Console.WriteLine(text);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                // if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                // {
                //     Console.WriteLine("Shutdown requested");
                //     runServer = false;
                // }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                // if (req.Url.AbsolutePath != "/favicon.ico")
                //     pageViews += 1;

                // Write the response info
                // string disableSubmit = !runServer ? "disabled" : "";

                byte[] data;
                var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                
                if (req.HttpMethod != "GET" || values == null || !values.ContainsKey("method"))
                {
                    data = Encoding.UTF8.GetBytes("404 Not Found");
                    resp.StatusCode = (int) HttpStatusCode.NotFound;
                    await resp.OutputStream.WriteAsync(data, 0, data.Length);
                    resp.Close();
                    
                    continue;
                }

                switch (values["method"])
                {
                    case "GetCurrentWeather":
                        text = WeatherReader.GetCurrentWeather(values["city"].ToString());
                        break;
                    default:
                        data = Encoding.UTF8.GetBytes("404 Not Found");
                        resp.StatusCode = (int) HttpStatusCode.NotFound;
                        await resp.OutputStream.WriteAsync(data, 0, data.Length);
                        resp.Close();
                    
                        continue;
                }
                
                data = Encoding.UTF8.GetBytes(text);

                resp.ContentType = "application/json";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }
        
        public static void Start()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();

        }
    }
}