using System.Net;
using Scrapper.Router;

namespace Scrapper.Listener;

public class AsyncListener : IAsyncListener
{
    private readonly HttpListener _listener;
    private readonly IRouter _router;

    private bool _isRunning;
        
    public AsyncListener(string[] prefixes, IRouter router)
    {
        _listener = new HttpListener();
        for (var i = 0; i < prefixes.Length; i++)
        {
            _listener.Prefixes.Add(prefixes[i]);
        }

        _router = router;
            
        _isRunning = false;
    }

    public void Schedule()
    {
        if (_isRunning)
        {
            return;
        }

        _isRunning = true;
        _listener.Start();
        _listener.BeginGetContext(ContextHandler, _listener);
    }

    public void Stop()
    {
        if (_isRunning == false)
        {
            return;
        }

        _isRunning = false;
        _listener.Stop();
    }

    private void ContextHandler(IAsyncResult result)
    {
        if (_isRunning == false)
        {
            return;
        }
            
        var listener = result.AsyncState as HttpListener;
        if (listener == null)
        {
            Stop();
            return;
        }

        var context = listener.EndGetContext(result);
        listener.BeginGetContext(ContextHandler, listener);
            
        var request = context.Request;
        var response = context.Response;

        _router.Route(request, response);
    }
}