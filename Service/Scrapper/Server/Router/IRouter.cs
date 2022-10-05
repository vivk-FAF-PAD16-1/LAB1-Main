using System.Net;

namespace Scrapper.Server.Router
{
    public interface IRouter
    {
        void Route(HttpListenerRequest request, HttpListenerResponse response);
    }
}