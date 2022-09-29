namespace Discovery.Listener
{
    public interface IAsyncListener
    {
        void Schedule();
        void Stop();
    }
}