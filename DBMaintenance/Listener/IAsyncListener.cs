namespace DBMaintenance.Listener;

public interface IAsyncListener
{
    void Schedule();
    void Stop();
}