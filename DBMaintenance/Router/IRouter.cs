using System.Net;

namespace DBMaintenance.Router;

public interface IRouter
{
    void Route(HttpListenerRequest request, HttpListenerResponse response);
}