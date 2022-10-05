using System.Net;

namespace Weather.Server.Router
{
    public interface IRouter
    {
        void Route(HttpListenerRequest request, HttpListenerResponse response);
    }
}