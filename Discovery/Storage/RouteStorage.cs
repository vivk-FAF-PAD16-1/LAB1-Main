using Discovery.Common;

namespace Discovery.Storage;

public class RouteStorage : IStorage
    {
        private Dictionary<string, DestinationContainer> _storage;
        
        private readonly object _locker;
        
        private const int DefaultCapacity = (1 << 5);

        public RouteStorage()
        {
            _locker = new object();
            lock (_locker)
            {
                _storage = new Dictionary<string, DestinationContainer>(DefaultCapacity);
            }
        }

        public void Register(string endpoint, string destinationUri)
        {
            var key = endpoint.TrimWeb();
                
            lock (_locker)
            {
                var contains = _storage.ContainsKey(key);
                DestinationContainer destinationContainer = null;
                
                if (contains == false)
                {
                    destinationContainer = new DestinationContainer();
                    _storage.Add(key, destinationContainer);
                }
                else
                {
                    destinationContainer = _storage[key];
                }
                
                destinationContainer.Add(destinationUri);
            }
        }
        
        public void Unregister(string endpoint, string destinationUri)
        {
            var key = endpoint.TrimWeb();
            
            lock (_locker)
            {
                var contains = _storage.ContainsKey(key);
                if (contains == false)
                {
                    return;
                }
                
                var destinationContainer = _storage[key];
                destinationContainer.Remove(destinationUri);
            }
        }

        public (bool, string) TryGet(string endpoint)
        {
            lock (_locker)
            {
                var (ok0, key, param) = TrySplitKeyParams(endpoint);
                if (ok0 == false)
                {
                    return (false, null);
                }

                var destinationContainer = _storage[key];
                var (ok1, uri) = destinationContainer.TryGet();
                if (ok1 == false)
                {
                    return (false, null);
                }

                return (true, uri + param);
            }
        }

        private (bool, string, string) TrySplitKeyParams(string endpoint)
        {
            var keys = _storage.Keys;
            foreach (var key in keys)
            {
                var valid = endpoint.StartWith(key);
                if (valid)
                {
                    var startIndex = key.Length;
                    var endIndex = endpoint.Length;
                    var param = startIndex == endIndex 
                        ? string.Empty 
                        : endpoint.Substring(startIndex, endIndex - startIndex);
                    
                    return (true, key, param);
                }
            }

            return (false, null, null);
        }
    }