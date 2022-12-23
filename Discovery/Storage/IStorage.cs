namespace Discovery.Storage;

public interface IStorage
{
    void Register(string endpoint, string destinationUri);
    void Unregister(string endpoint, string destinationUri);

    (bool, string) TryGet(string endpoint);
}