using System.Threading.Tasks;

namespace Gateway.Storage.Interfaces
{
	public interface IStorageSender
	{
		Task<string> SendMessage(string categoryPath, string message);
	}
}