namespace Scrapper.Listener;

public interface IAsyncListener
{
    void Schedule();
    void Stop();
}