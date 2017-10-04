using System;
using Foundation;
using UIKit;
using Plugin.SimpleAudioPlayer;

namespace SAPlayerSample.tvOS
{
    public partial class ViewController : UIViewController
    {
        Plugin.SimpleAudioPlayer.Abstractions.ISimpleAudioPlayer player;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            player = CrossSimpleAudioPlayer.Current;
            player.Load("Diminished.mp3");
        }

        partial void PlayButton_PrimaryActionTriggered(UIButton sender)
        {
            player.Play();

            isPlaying.Text = "Playing";
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        partial void StopButton_PrimaryActionTriggered(UIButton sender)
        {
            player.Stop();

            isPlaying.Text = "Stopped";
        }
    }
}

