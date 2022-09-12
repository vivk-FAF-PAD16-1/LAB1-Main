using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Gateway.Storage
{
    public struct CategoryData
    {
        private List<SubData> _subscribers;

        private readonly string _categoryPath;

        private readonly HttpClient _client;
        private readonly object _locker;

        private const int DefaultCapacity = 32;

        public CategoryData(string categoryPath)
        {
            _subscribers = new List<SubData>(capacity: DefaultCapacity);
            
            _categoryPath = categoryPath;

            _client = new HttpClient();
            _locker = new object();
        }

        public string GetCategoryPath()
        {
            return _categoryPath;
        }

        public void Add(string ip, int port)
        {
            lock (_locker)
            {
                for (var i = 0; i < _subscribers.Count; i++)
                {
                    var sub = _subscribers[i];
                    if (sub.GetIp() == ip && sub.GetPort() == port)
                    {
                        return;
                    }
                }
                
                var newSub = new SubData(ip, port);
                _subscribers.Add(newSub);
            }
        }

        public void Remove(string ip, int port)
        {
            lock (_locker)
            {
                for (var i = 0; i < _subscribers.Count; i++)
                {
                    var sub = _subscribers[i];
                    if (sub.GetIp() != ip || sub.GetPort() != port)
                    {
                        continue;
                    }

                    _subscribers.RemoveAt(i);
                    return;
                }
            }
        }

        public async Task<string> SendMessage(string message)
        {
            var (subData, ok) = LoadBalance();
            if (ok == false)
            {
                // TODO: LOG EMPTY CATEGORY
                return null;
            }

            var serviceAddr = $"{subData.GetIp()}:{subData.GetPort()}";
            var response = await _client.GetStringAsync(serviceAddr);

            return response;
        }

        private (SubData, bool) LoadBalance()
        {
            lock (_locker)
            {
                if (_subscribers.Count == 0)
                {
                    return (default, false);
                }

                var currentSub = _subscribers[0];
                
                for (var i = 1; i < _subscribers.Count; i++)
                {
                    var sub = _subscribers[i];
                    if (sub.GetOperations() < currentSub.GetOperations())
                    {
                        currentSub = sub;
                    }
                }

                return (currentSub, true);
            }
        }
    }
}