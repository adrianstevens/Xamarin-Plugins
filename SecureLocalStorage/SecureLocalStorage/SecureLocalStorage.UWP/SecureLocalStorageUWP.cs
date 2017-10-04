using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using System.IO;
using System.Threading.Tasks;

namespace SecureLocalStorage
{
    public class SecureLocalStorageUWP : SecureStorage
    {
        const string FILENAME = "file.save";

        public override async Task<bool> SaveAsync()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(DictionaryToJson(data));

            var provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider("LOCAL=user");

            var cipher = (await provider.ProtectAsync(bytes.AsBuffer()).AsTask().ConfigureAwait(false)).ToArray();
                
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync(FILENAME, CreationCollisionOption.OpenIfExists).AsTask().ConfigureAwait(false);

            using (var stream = await file.OpenStreamForWriteAsync().ConfigureAwait(false))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write((Int32)cipher.Length);
                    writer.Write(cipher);
                }
            }
            return true;
        }

        public override async Task<bool> LoadAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(FILENAME);

            var provider = new Windows.Security.Cryptography.DataProtection.DataProtectionProvider("LOCAL=user");

            using (var stream = await file.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var length = reader.ReadInt32();
                    var cipher = reader.ReadBytes(length);

                    var plaintext = await provider.UnprotectAsync(cipher.AsBuffer()).AsTask().ConfigureAwait(false);
                    var bytes = plaintext.ToArray();

                    var json = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    data = JsonToDictionary(json);
                }
            }

            return true;
        }
    }
}