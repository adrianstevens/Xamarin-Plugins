using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SecureLocalStorage.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new SecureLocalStorage.App());

            SaveAndLoad();

            /*
            var json = ss.DictionaryToJson(ss.data);

            var d = ss.JsonToDictionary(json);

            for (int i = 0; i < d.Count; i++)
            {
                Debug.Assert(d.Keys.ToList()[i] == ss.Keys[i]);
                Debug.Assert(d.Values.ToList()[i] == ss.Values[i]);
            } */
        }

        async Task SaveAndLoad ()
        {
            var ss = new SecureLocalStorageUWP();

            for (int i = 0; i < 99; i++)
                ss.Add($"key{i}", $"value!@#$%^&^*()!<>?`~@#$:;'\"{i}");

            //saving the original data
            var json = ss.DictionaryToJson(ss.data);
            var d = ss.JsonToDictionary(json);

            //end 

            await ss.SaveAsync();

            ss.Clear();

            await ss.LoadAsync();



            for (int i = 0; i < d.Count; i++)
            {
                Debug.Assert(d.Keys.ToList()[i] == ss.Keys[i]);
                Debug.Assert(d.Values.ToList()[i] == ss.Values[i]);
            }

            int pause = 0;






        }
    }
}