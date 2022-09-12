using System;
using System.Net;

namespace Gateway.Storage
{
    public struct SubData : IEquatable<SubData>
    {
        private string _ip;
        private int _port;
        
        private int _currentOperations;

        private readonly object _locker;

        public SubData(string ip, int port)
        {
            _ip = ip;
            _port = port;
            
            _currentOperations = 0;
            
            _locker = new object();
        }

        public string GetIp()
        {
            return _ip;
        }
        
        public int GetPort()
        {
            return _port;
        }
        
        public int GetOperations()
        {
            lock (_locker)
            {
                return _currentOperations;
            }
        }

        public void Register()
        {
            lock (_locker)
            {
                _currentOperations++;
            }
        }

        public void Unregister()
        {
            lock (_locker)
            {
                _currentOperations--;
            }
        }

        public bool Equals(SubData other)
        {
            return _ip == other._ip && 
                   _port == other._port;
        }

        public override bool Equals(object obj)
        {
            return obj is SubData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_ip != null ? _ip.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _port;
                hashCode = (hashCode * 397) ^ _currentOperations;
                hashCode = (hashCode * 397) ^ (_locker != null ? _locker.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}