namespace Gateway.Storage.Interfaces
{
	public interface IStorageRegistrator
	{
		void Register(string categoryPath, string ip, int port);
	}
}