using AVFoundation;
using Foundation;
using System;
using System.IO;

namespace Plugin.SimpleAudioPlayer
{
  /// <summary>
  /// Implementation for SimpleAudioPlayer
  /// </summary>
  public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
  {
		///<Summary>
		/// Raised when playback completes or loops
		///</Summary>
		public event EventHandler PlaybackEnded;

        AVAudioPlayer player;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration => player == null ? 0 : player.Duration;

        ///<Summary>
        /// Current position of audio in seconds
        ///</Summary>
        public double CurrentPosition => player == null ? 0 : player.CurrentTime;

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume
        {
            get => player == null ? 0 : player.Volume;
            set => SetVolume(value, Balance);
        }

        ///<Summary>
        /// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
        ///</Summary>
        public double Balance
        {
            get => _balance;
            set => SetVolume(Volume, _balance = value);
        }
        double _balance = 0;

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        public bool IsPlaying => player != null && player.Playing;

        ///<Summary>
        /// Continously repeats the currently playing sound
        ///</Summary>
        public bool Loop
        {
            get => _loop;
            set
            {
                _loop = value;
                if (player != null)
                {
                    player.NumberOfLoops = _loop ? -1 : 0;
                }
            }
        }
        bool _loop;

        ///<Summary>
        /// Indicates if the position of the loaded audio file can be updated - always returns true on iOS
        ///</Summary>
        public bool CanSeek => player != null;

        ///<Summary>
        /// Load wave or mp3 audio file as a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            DeletePlayer();

            var data = NSData.FromStream(audioStream);

            player = AVAudioPlayer.FromData(data);

            return PreparePlayer();
        }

        ///<Summary>
        /// Load wave or mp3 audio file from the Android assets folder
        ///</Summary>
        public bool Load(string fileName)
        {
            DeletePlayer();

            player = AVAudioPlayer.FromUrl(NSUrl.FromFilename(fileName));

            return PreparePlayer();
        }

        bool PreparePlayer()
        {
            if (player != null)
            {
                player.FinishedPlaying += OnPlaybackEnded;
                player.PrepareToPlay();
            }

            return player != null;
        }

        void DeletePlayer()
        {
            Stop();

            if(player != null)
            {
                player.FinishedPlaying -= OnPlaybackEnded;
                player.Dispose();
                player = null;
            }
        }

        private void OnPlaybackEnded(object sender, AVStatusEventArgs e)
        {
            PlaybackEnded?.Invoke(sender, e);
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
        public void Play()
        {
            if (player == null)
                return;

            if (player.Playing)
                player.CurrentTime = 0;
            else
                player?.Play();
        }

        ///<Summary>
        /// Pause playback if playing (does not resume)
        ///</Summary>
        public void Pause()
        {
            player?.Pause();
        }

        ///<Summary>
        /// Stop playack and set the current position to the beginning
        ///</Summary>
        public void Stop()
        {
            player?.Stop();
            Seek(0);
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
        }

        ///<Summary>
        /// Seek a position in seconds in the currently loaded sound file 
        ///</Summary>
        public void Seek (double position)
        {
            if (player == null)
                return;
            player.CurrentTime = position;
        }

        void SetVolume(double volume, double balance)
        {
            if (player == null)
                return;

            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            balance = Math.Max(-1, balance);
            balance = Math.Min(1, balance);

            player.Volume = (float)volume;
            player.Pan = (float)balance;
        }

        bool isDisposed = false;
		///<Summary>
		/// Dispose SimpleAudioPlayer and release resources
		///</Summary>
		protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            if (disposing)
                DeletePlayer();

            isDisposed = true;
        }

        ~SimpleAudioPlayerImplementation()
        {
            Dispose(false);
        }

		///<Summary>
		/// Dispose SimpleAudioPlayer and release resources
		///</Summary>
		public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}