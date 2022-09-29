using System;

namespace Weather.Server.Listener
{
    public interface IAsyncListener
    {
        void Schedule();
        void Stop();
    }
}