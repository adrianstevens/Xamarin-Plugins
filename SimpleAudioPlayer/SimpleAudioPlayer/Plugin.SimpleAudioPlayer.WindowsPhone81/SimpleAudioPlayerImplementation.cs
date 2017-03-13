using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Plugin.SimpleAudioPlayer
{
  /// <summary>
  /// Implementation for SimpleAudioPlayer
  /// </summary>
  public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
  {
        public event EventHandler PlaybackEnded;

        MediaElement player;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration
        { get { return player == null ? 0 : player.NaturalDuration.TimeSpan.TotalSeconds; } }

        ///<Summary>
        /// Current position of audio in seconds
        ///</Summary>
        public double CurrentPosition
        { get { return player == null ? 0 : player.Position.TotalSeconds; } }

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume
        {
            get { return player == null ? 0 : player.Volume; }
            set { SetVolume(value, Balance); }
        }

        ///<Summary>
        /// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
        ///</Summary>
        public double Balance
        {
            get { return _balance; }
            set { SetVolume(Volume, _balance = value); }
        }
        double _balance = 0;

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        public bool IsPlaying
        {
            get
            {
                if (player == null)
                    return false;
                return player.CurrentState == MediaElementState.Playing; //might need to expand
            }
        }

        ///<Summary>
        /// Indicates if the position of the loaded audio file can be updated
        ///</Summary>
        public bool CanSeek
        { get { return player == null ? false : player.CanSeek; } }

        ///<Summary>
        /// Load wave or mp3 audio file as a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            if (player == null)
                player = new MediaElement() { AutoPlay = false };

            if (player != null)
            {
                player.SetSource(audioStream.AsRandomAccessStream(), "");
                player.MediaEnded += OnPlaybackEnded;
            }

            return (player == null) ? false : true;
        }

        ///<Summary>
        /// Load wave or mp3 audio file from Assets folder
        ///</Summary>
        public bool Load(string fileName)
        {
            if (player == null)
                player = new MediaElement() { AutoPlay = false };

            //a bit ugly but Windows 8x support in Forms will probably go away
            var folder = Package.Current.InstalledLocation.GetFolderAsync("Assets").AsTask().Result;
            var file = folder.GetFileAsync(fileName).AsTask().Result;

            var stream = file.OpenAsync(FileAccessMode.Read).AsTask().Result;

            if (player != null)
            {
                player.SetSource(stream, "");
                player.MediaEnded += OnPlaybackEnded;
            }

            return (player == null) ? false : true;
        }

        private void OnPlaybackEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            PlaybackEnded?.Invoke(sender, EventArgs.Empty);
        }

        public void Play()
        {
            if (player == null)
                return;

            if (player.CurrentState == MediaElementState.Playing)
            {
                player.Stop();
                player.Play();
            }
            else
            {
                player.Play();
            }
        }

        public void Pause()
        {
            player?.Pause();
        }
        public void Stop()
        {
            if (player != null)
            {
                player.Stop();
                player.Position = TimeSpan.Zero;
            }
        }

        public void Seek(double position)
        {
            if (player != null && player.CanSeek)
                player.Position = TimeSpan.FromSeconds(position);
        }

        void SetVolume(double volume, double balance)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            balance = Math.Max(0, balance);
            balance = Math.Min(1, balance);

            var right = (balance < 0) ? volume * -1 * balance : volume;
            var left = (balance > 0) ? volume * 1 * balance : volume;

            player.Volume = volume;
        }

        public void Dispose()
        {
            //MediaElement does not require cleanup
        }
    }
}