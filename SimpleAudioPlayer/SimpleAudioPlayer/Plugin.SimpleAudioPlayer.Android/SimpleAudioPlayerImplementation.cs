using Android.Content.Res;
using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;

namespace Plugin.SimpleAudioPlayer
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
    {
        ///<Summary>
        /// Raised when audio playback completes successfully 
        ///</Summary>
        public event EventHandler PlaybackEnded;

        Android.Media.MediaPlayer player;

        static int index = 0;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration
        { get { return player == null ? 0 : ((double)player.Duration) / 1000.0; } }

        ///<Summary>
        /// Current position of audio playback in seconds
        ///</Summary>
        public double CurrentPosition
        { get { return player == null ? 0 : ((double)player.CurrentPosition) / 1000.0; } }

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume
        {
            get { return _volume; }
            set { SetVolume(_volume = value, Balance); }
        }
        double _volume = 0.5;

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
        { get { return player == null ? false : player.IsPlaying; } }

        ///<Summary>
        /// Indicates if the position of the loaded audio file can be updated
        ///</Summary>
        public bool CanSeek
        { get { return player == null ? false : true; } }

        string path;

        ///<Summary>
        /// Load wav or mp3 audio file as a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            //cache to the file system
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"cache{index++}.wav");
            var fileStream = File.Create(path);
            audioStream.CopyTo(fileStream);
            fileStream.Close();

            //load the cached audio into MediaPlayer
            player?.Dispose();
            player = new Android.Media.MediaPlayer();
            player?.SetDataSource(path);
            player?.Prepare();

            if(player != null)
                player.Completion += OnPlaybackEnded;

            return (player == null) ? false : true;
        }

        ///<Summary>
        /// Load wav or mp3 audio file from the iOS Resources folder
        ///</Summary>
        public bool Load(string fileName)
        {
            AssetFileDescriptor afd = Android.App.Application.Context.Assets.OpenFd(fileName);

            player?.Dispose();
            player = new Android.Media.MediaPlayer();

            player?.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            
            player?.Prepare();

            if (player != null)
                player.Completion += OnPlaybackEnded;

            return (player == null) ? false : true;
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
        public void Play()
        {
            if (player == null)
                return;

            if (player.IsPlaying)
            {
                player.Pause();
                player.SeekTo(0);
            }

            player.Start();
        }

        ///<Summary>
        /// Stop playack and set the current position to the beginning
        ///</Summary>
        public void Stop()
        {
            player?.Pause();
            player?.SeekTo(0);
        }

        ///<Summary>
        /// Pause playback if playing (does not resume)
        ///</Summary>
        public void Pause()
        {
            player?.Pause();
        }

        ///<Summary>
        /// Set the current playback position (in seconds)
        ///</Summary>
        public void Seek (double position)
        {
            if (player != null)
                player.SeekTo((int)position*1000);
        }

        ///<Summary>
        /// Sets the playback volume as a double between 0 and 1
        /// Sets both left and right channels
        ///</Summary>
        void SetVolume(double volume, double balance)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            balance = Math.Max(0, balance);
            balance = Math.Min(1, balance);

            var right = (balance < 0) ? volume * -1 * balance : volume;
            var left = (balance > 0) ? volume * 1 * balance : volume;

            player.SetVolume((float)left, (float)right);
        }

        void OnPlaybackEnded(object sender, EventArgs e)
        {
            PlaybackEnded?.Invoke(sender, e);
        }

        bool isDisposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed || player == null)
                return;

            if (disposing)
            {
                if (IsPlaying)
                    player.Stop();

                player.Completion -= OnPlaybackEnded;

                player.Dispose();
            }
            player = null;

            isDisposed = true;

            if (string.IsNullOrWhiteSpace(path) == false)
            {
                try
                {
                    File.Delete(path);
                    path = string.Empty;
                }
                catch
                {
                }
            }

            isDisposed = true;
        }

        ~SimpleAudioPlayerImplementation()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}