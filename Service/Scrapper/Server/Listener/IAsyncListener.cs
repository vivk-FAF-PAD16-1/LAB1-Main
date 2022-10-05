namespace Scrapper.Server.Listener
{
    public interface IAsyncListener
    {
        void Schedule();
        void Stop();
    }
}