using System.Collections.Generic;
using System.Threading.Tasks;
using Gateway.Storage.Interfaces;

namespace Gateway.Storage
{
    public class Storage : IStorageRegistrator, IStorageSender
    {
        private readonly Dictionary<string, CategoryData> _categories;
        
        private const int DefaultCapacity = (1 << 5);

        public Storage()
        {
            _categories = new Dictionary<string, CategoryData>(DefaultCapacity);
        }

        public void Register(string categoryPath, string ip, int port)
        {
            var existCategory = _categories.ContainsKey(categoryPath);
            if (existCategory == false)
            {
                var newCategory = new CategoryData(categoryPath);
                _categories.Add(categoryPath, newCategory);
            }

            var category = _categories[categoryPath];
            category.Add(ip, port);
        }

        public async Task<string> SendMessage(string categoryPath, string message)
        {
            var existCategory = _categories.ContainsKey(categoryPath);
            if (existCategory == false)
            {
                return null;
            }

            var category = _categories[categoryPath];
            var response = await category.SendMessage(message);
            return response;
        }
    }
}