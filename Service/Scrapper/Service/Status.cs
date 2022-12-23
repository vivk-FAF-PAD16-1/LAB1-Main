namespace Scrapper.Service;

public class Status
{
    private DateTime _dataTime;
    private int _numReceivedCalls;
    private int _numAcceptedCalls;
    private int _numSentCalls;
        
    private readonly object _locker;

    public Status()
    {
        _dataTime = DateTime.Now;
        _numAcceptedCalls = 0;
        _numReceivedCalls = 0;
        _numSentCalls = 0;
            
        _locker = new object();
    }

    public void OnCallReceived()
    {
        lock (_locker)
        {
            _numAcceptedCalls += 1;
        }
    }
        
    public void OnCallAccepted()
    {
        lock (_locker)
        {
            _numReceivedCalls += 1;
        }
    }
        
    public void OnCallSent()
    {
        lock (_locker)
        {
            _numSentCalls += 1;
        }
    }

    public string GetStatus()
    {
        return "{\n" +
               "\"time_up:\" \"" + (DateTime.Now - _dataTime) + "\",\n" +
               "\"accepted_calls:\" \"" + _numAcceptedCalls + "\",\n" +
               "\"received_calls:\" \"" + _numReceivedCalls + "\",\n" +
               "\"sent_calls:\" \"" + _numSentCalls + "\"\n" +
               "}";
    }
}