
using System.Net;
using System.Text;

namespace DBMaintenance.Common;

public static class HttpUtilities
{
    public static string ReadRequestBody(HttpListenerRequest request)
    {
        var body = request.InputStream;
        var encoding = request.ContentEncoding;

        var reader = new StreamReader(body, encoding);
        var content = reader.ReadToEnd();

        reader.Close();
        body.Close();

        return content;
    }

    public static string ReadResponseBody(HttpResponseMessage response)
    {
        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }

    public static void SendResponseMessage(HttpListenerResponse response, string message, int statusCode = 200)
    {
        var buffer = Encoding.UTF8.GetBytes(message);

        response.ContentEncoding = Encoding.UTF8;
        response.ContentLength64 = buffer.Length;
        response.StatusCode = statusCode;

        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    private const string NotFoundMessage = "404 NOT FOUND!";
    private const int NotFoundStatus = 404;

    public static void NotFoundResponse(HttpListenerResponse response)
    {
        var buffer = Encoding.UTF8.GetBytes(NotFoundMessage);

        response.ContentEncoding = Encoding.UTF8;
        response.ContentLength64 = buffer.Length;
        response.StatusCode = NotFoundStatus;

        var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
    }

    private static readonly char[] WebCharacters = { '/', '\\' };

    public static string TrimWeb(this string source)
    {
        return source.Trim(WebCharacters);
    }

    public static bool StartWith(this string source, string find)
    {
        var length = find.Length;
        if (length > source.Length)
        {
            return false;
        }

        for (var i = 0; i < length; i++)
        {
            if (find[i] != source[i])
            {
                return false;
            }
        }

        if (length != source.Length)
        {
            var webCharacter = source[length];
            if (WebCharacters.Contains(webCharacter) == false)
            {
                return false;
            }
        }

        return true;
    }
}
