using System;
using Xamarin.Forms;
using Plugin.SimpleAudioRecorder;
using Plugin.SimpleAudioPlayer;
using Plugin.SimpleAudioPlayer.Abstractions;

namespace XFSpeechToText
{
	public partial class MainPage : ContentPage
	{
        ISimpleAudioRecorder audioRecorder;
        AudioRecording audioRecording;

        ISimpleAudioPlayer audioPlayer;

        public MainPage()
		{
			InitializeComponent();

            audioRecorder = CrossSimpleAudioRecorder.CreateSimpleAudioRecorder();
            audioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();

            btnRecord.Clicked += BtnRecord_Clicked;
            btnStop.Clicked += BtnStop_Clicked;
            btnPlay.Clicked += BtnPlay_Clicked;
            btnToText.Clicked += BtnToText_Clicked;
		}

        private void BtnToText_Clicked(object sender, EventArgs e)
        {
            
        }

        private void BtnPlay_Clicked(object sender, EventArgs e)
        {
            var stream = audioRecording.GetAudioStream();

            audioPlayer.Load(stream);

            audioPlayer.Play();
        }

        private async void BtnStop_Clicked(object sender, EventArgs e)
        {
            audioRecording = await audioRecorder.StopAsync();
        }

        private async void BtnRecord_Clicked(object sender, EventArgs e)
        {
            try
            {
                await audioRecorder.RecordAsync();
            }
            catch
            {

            }
        }
    }
}
