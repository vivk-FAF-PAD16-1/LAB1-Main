using System.Net;

namespace Discovery.Router;

public interface IRouter
{
    void Route(HttpListenerRequest request, HttpListenerResponse response);
}