using System.Net;

namespace Gateway.Router
{
    public interface IRouter
    {
        void Route(HttpListenerRequest request, HttpListenerResponse response);
    }
}