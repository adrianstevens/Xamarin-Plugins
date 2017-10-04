using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* New Account
 * enter user/pass
 * PBKDF password to create key and store in memory only
 * Use key to encrypt/descrypt
 * save known awesome message? (if the attacker know the plain text ... it's easier to attack)
 * 
 */
 
/* V2 security
 * 1. user enters password every time the app is launched
 * 2. password is hashed and validated against stored salted hash
 * 3. password is used with PBKDF which generates a "good" key
 * 4. PBKDF key is used to decrypt stored (encryped) key material for AES
 * 5. User data is decrypted using key material (and good IV practices)
 * 
 * 
 * For new Account
 * 1. Enter username/password (username saved, password is not)
 * 2. Use PBKDF to generate a good key (never saved)
 * 3. Generate IV + key material for local storage 
 * 4. Hash password <- will be encrypted by PBKDF key and saved - leaking the hash is only a problem for weak passwords (12+ chars)
 * 5. Encrypt both hash and IV + key material and store to disk (password is never saved)
 * 6. Use IV + key material to encrypt and store user data as needed (i.e. diary data in SQLite)
 */ 

namespace SecureLocalStorage
{
    public abstract class SecureStorage : ISecureStorage
    {
        public Dictionary<string, string> data = new Dictionary<string, string>();

        public string this[string key] { get => data[key]; set => data[key] = value; }

        public IList<string> Keys => data.Keys.ToList();

        public IList<string> Values => data.Values.ToList();

        public int Count => data.Count;

        public abstract Task<bool> LoadAsync();

        public abstract Task<bool> SaveAsync();

        public void Add(string key, string value)
        {
            data.Add(key, value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public bool ContainsValue(string value)
        {
            return data.ContainsValue(value);
        }

        public void Remove(string key)
        {
            data.Remove(key);
        }

        public string DictionaryToJson(Dictionary<string, string> dictionary)
        {
            var entries = dictionary.Select(entry => $"\"{Uri.EscapeDataString(entry.Key)}\":\"{Uri.EscapeDataString(entry.Value)}\"");

            var join = string.Join(",", entries);
            var json = "{" + join + "}";

            return json;
        }

        public Dictionary<string, string> JsonToDictionary(string json)
        {
            var result = json;
            result = result.Replace("{", string.Empty);
            result = result.Replace("}", string.Empty);
            result = result.Replace("\"", string.Empty);

            var entries = result.Split(',');

            var dictionary = entries.ToDictionary(entry => Uri.UnescapeDataString(entry.Split(':')[0]), entry => Uri.UnescapeDataString(entry.Split(':')[1]));
            return dictionary;
        }
      
        public bool TryGetValue(string key, out string value)
        {
            return data.TryGetValue(key, out value);
        }
    }
}