namespace Gateway.Listener
{
    public interface IAsyncListener
    {
        void Schedule();
        void Stop();
    }
}