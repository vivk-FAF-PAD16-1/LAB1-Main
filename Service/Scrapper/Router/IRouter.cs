using System.Net;

namespace Scrapper.Router;

public interface IRouter
{
    void Route(HttpListenerRequest request, HttpListenerResponse response);
}