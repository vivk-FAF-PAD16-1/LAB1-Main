using System.IO;
using System.Net;

namespace Scrapper.Common
{
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
    }
}