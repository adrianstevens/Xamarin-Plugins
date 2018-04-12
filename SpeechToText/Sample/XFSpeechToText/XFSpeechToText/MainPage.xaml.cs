using System;
using Xamarin.Forms;
using Plugin.SpeechToText;
using Plugin.SimpleAudioRecorder;
using Plugin.SimpleAudioPlayer;
using Plugin.SimpleAudioPlayer.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

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

            CheckPermissions();
		}

        async Task CheckPermissions()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);

                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                {
                    await DisplayAlert("Need Microphone", "This app needs access to the microphone to record audio", "OK");

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Microphone);
                    
                    if (results.ContainsKey(Permission.Microphone))
                        status = results[Permission.Microphone];

                    if (status == PermissionStatus.Granted)
                    {
                        
                    }
                }
            }
            catch
            {

            }
        }

        private async void BtnToText_Clicked(object sender, EventArgs e)
        {
            var speech = new SpeechToText("{bing speech api key goes here");

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
