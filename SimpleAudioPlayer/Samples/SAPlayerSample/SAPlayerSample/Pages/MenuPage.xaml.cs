using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace SAPlayerSample
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();

            btnPCL.Clicked += (s, e) => Navigation.PushAsync(new PCLAudioPage());
            btnLocal.Clicked += (s, e) => Navigation.PushAsync(new HeadProjectAudioPage());
        }
    }
}
