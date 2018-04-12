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

        private async void BtnToText_Clicked(object sender, EventArgs e)
        {
            var speech = new SpeechToText.SpeechToText("fb3c4e67a81242f794cb56ebb279271d");

            var result = await speech.RecognizeSpeechAsync(audioRecording.GetFilePath());

            System.Diagnostics.Debug.WriteLine(lblText.Text = result.DisplayText);
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
