using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Linq;

namespace SecureLocalStorage.Droid
{
    [Activity(Label = "SecureLocalStorage", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            SaveAndLoad();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

        async Task SaveAndLoad()
        {
            var ss = new SecureLocalStorageAndroid(this);

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
                System.Diagnostics.Debug.Assert(d.Keys.ToList()[i] == ss.Keys[i]);
                System.Diagnostics.Debug.Assert(d.Values.ToList()[i] == ss.Values[i]);
            }

            int pause = 0;
        }
    }
}