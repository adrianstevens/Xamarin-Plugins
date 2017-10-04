using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureLocalStorage
{
    public interface ISecureStorage
    {
        string this[string key] { get; set; }

        IList<string> Keys { get; }
        IList<string> Values { get; }

        int Count { get; }


        void Add(string key, string value);
        bool ContainsKey(string key);
        bool ContainsValue(string value);
        
        bool TryGetValue(string key, out string value);

        void Remove(string key);

        void Clear();
                

        Task<bool> SaveAsync();
        Task<bool> LoadAsync();
    }
}