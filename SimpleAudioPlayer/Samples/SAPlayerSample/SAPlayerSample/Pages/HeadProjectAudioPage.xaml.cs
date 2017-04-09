using System;

using Xamarin.Forms;

namespace SAPlayerSample
{
    public partial class HeadProjectAudioPage : ContentPage
    {
        public HeadProjectAudioPage()
        {
            InitializeComponent();

            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

            player.Load("running.mp3");


            btnPlay.Clicked += BtnPlayClicked;
            switchLoop.Toggled += SwitchLoopToggled;
        }

        private void SwitchLoopToggled(object sender, ToggledEventArgs e)
        {
            Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current.Loop = switchLoop.IsToggled;
        }

        private void BtnPlayClicked(object sender, EventArgs e)
        {
            Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current.Play();
        }
    }
}
