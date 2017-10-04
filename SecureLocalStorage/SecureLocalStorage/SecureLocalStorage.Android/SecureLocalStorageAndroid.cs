using Android.Content;
using Java.Security;
using Javax.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;

namespace SecureLocalStorage
{
    class SecureLocalStorageAndroid : SecureStorage
    {
        static readonly object fileLock = new object();

        Context context => Android.App.Application.Context;

        const string FILENAME = "file.save";
        const string PREFERENCES_KEY = "Settings";

        public SecureLocalStorageAndroid(Context context)
        {
            //this.context = context;
        }

        public override Task<bool> LoadAsync()
        {
            var secureKey = GetKeyFromPreferences(context);
            var keyStore = LoadKeyStore(context, secureKey);
            var password = new KeyStore.PasswordProtection(secureKey);

            var entry = keyStore.GetEntry(FILENAME, password) as KeyStore.SecretKeyEntry;

            if (entry != null)
            {
                var bytes = entry.SecretKey.GetEncoded();

                var json = System.Text.Encoding.UTF8.GetString(bytes);

                data = JsonToDictionary(json);
            }

            return Task.FromResult(true);
        }

        public override Task<bool> SaveAsync()
        {
            var secureKey = GetKeyFromPreferences(context);
            var keyStore = LoadKeyStore(context, secureKey);
            var password = new KeyStore.PasswordProtection(secureKey);

            var secretValue = new SecretValue(System.Text.Encoding.UTF8.GetBytes(DictionaryToJson(data)));

            var secretKeyEntry = new KeyStore.SecretKeyEntry(secretValue);
            keyStore.SetEntry(FILENAME, secretKeyEntry, password);

            lock (fileLock)
            {
                using (var stream = context.OpenFileOutput(FILENAME, FileCreationMode.Private))
                {
                    keyStore.Store(stream, secureKey);
                    stream.Flush();
                    stream.Close();
                }
            }
            return Task.FromResult(true);
        }

        KeyStore LoadKeyStore(Context context, char[] secureKey)
        {
            var keyStore = KeyStore.GetInstance(KeyStore.DefaultType);

            try
            {
                lock (fileLock)
                {
                    if (context.GetFileStreamPath(FILENAME)?.Exists() ?? false)
                    {
                        using (var s = context.OpenFileInput(FILENAME))
                            keyStore.Load(s, secureKey); //decrypt (probably)
                    }
                    else
                    {
                        keyStore.Load(null, secureKey);
                    }
                }
            }
            catch
            {
                keyStore.Load(null, secureKey);
            }

            return keyStore;
        }

       
        // The secure key will be created if it doesn't exist the first time it's needed
        // We then store the key in the private shared preferences so only this app has access ... is this secure???? (nope)

        /*
         "Anyone with root level access to the device will be able to see them, 
         as root has access to everything on the filesystem. Also, any application 
         that runs with the same UID as the creating app would be able to access 
         them (this is not usually done and you need to take specific action to 
         make two apps runs with the same UID, so this is probably not a big concern)."
         //https://stackoverflow.com/questions/9244318/android-sharedpreference-security

        //hash discussion: https://nakedsecurity.sophos.com/2013/11/20/serious-security-how-to-store-your-users-passwords-safely/

        */

         //prefer byte[]
        static char[] GetKeyFromPreferences(Context context)
        {
            const string CACHEKEY_KEY = "CacheKey";

            var cacheKey = string.Empty;
            var prefs = context.GetSharedPreferences(PREFERENCES_KEY, FileCreationMode.Private);

            if (prefs.Contains(CACHEKEY_KEY))
            {
                cacheKey = prefs.GetString(CACHEKEY_KEY, string.Empty);
  
                //found the cached key
                if (!string.IsNullOrEmpty(cacheKey))
                    return cacheKey.ToCharArray();
            }

            // Generate a 256-bit key
            const int keyLength = 256;

            // Do not seed SecureRandom - automatically seeded from system entropy
            var secureRandom = new SecureRandom();

            var keyGen = KeyGenerator.GetInstance("AES");
            keyGen.Init(keyLength, secureRandom);

            var key = keyGen.GenerateKey();
            cacheKey = Convert.ToBase64String(key.GetEncoded());

            prefs.Edit().PutString(CACHEKEY_KEY, cacheKey).Commit();

            return cacheKey.ToCharArray();
        }


        class SecretValue : Java.Lang.Object, ISecretKey
        {
            byte[] value;

            public string Algorithm => "RAW";

            public string Format => "RAW";

            public SecretValue(byte[] value)
            {
                this.value = value;
            }

            public byte[] GetEncoded()
            {
                return value;
            }
        }

        class SecretEntry : Java.Lang.Object, ISecretKey
        {
            byte[] bytes;

            public SecretEntry(string value)
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(value);
            }

            public string Algorithm { get { return "RAW"; } }

            public string Format { get { return "RAW"; } }

            public byte[] GetEncoded()
            {
                return bytes;
            }
        }
    }
}
