using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;
using System.Windows.Media;

namespace Plugin.SimpleAudioPlayer
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
    {
        public event EventHandler PlaybackEnded;

        private static int _index;

        private MediaPlayer _player;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration => _player?.NaturalDuration.TimeSpan.TotalSeconds ?? 0;

        ///<Summary>
        /// Current position of audio in seconds
        ///</Summary>
        public double CurrentPosition => _player?.Position.TotalSeconds ?? 0;

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume
        {
            get => _player?.Volume ?? 0;
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

        private double _balance;

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        public bool IsPlaying
        {
            get
            {
                if (_player == null)
                    return false;
                return _isPlaying;
            }
        }

        private bool _isPlaying;

        ///<Summary>
        /// Continously repeats the currently playing sound
        ///</Summary>
        public bool Loop
        {
            get => _loop;
            set => _loop = value;
        }

        private bool _loop;

        ///<Summary>
        /// Indicates if the position of the loaded audio file can be updated
        ///</Summary>
        public bool CanSeek => _player != null;

        ///<Summary>
        /// Load wave or mp3 audio file from a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            DeletePlayer();

            _player = GetPlayer();

            if (_player != null)
            {
                var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                    $"{++_index}.wav");
                using (var fileStream = File.OpenWrite(fileName)) audioStream.CopyTo(fileStream);

                _player.Open(new Uri(fileName));
                _player.MediaEnded += OnPlaybackEnded;
            }

            return _player != null && _player.Source != null;
        }

        ///<Summary>
        /// Load wave or mp3 audio file from assets folder in the UWP project
        ///</Summary>
        public bool Load(string fileName)
        {
            DeletePlayer();

            _player = GetPlayer();

            if (_player != null)
            {
                _player.Open(new Uri("ms-appx:///Assets/" + fileName));
                _player.MediaEnded += OnPlaybackEnded;
            }

            return _player != null && _player.Source != null;
        }

        void DeletePlayer()
        {
            Stop();

            if (_player != null)
            {
                _player.MediaEnded -= OnPlaybackEnded;
                _player = null;
            }
        }

        private void OnPlaybackEnded(object sender, EventArgs args)
        {
            if (_isPlaying && _loop)
            {
                Play();
            }

            PlaybackEnded?.Invoke(sender, args);
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
        public void Play()
        {
            if (_player == null || _player.Source == null)
                return;

            if (IsPlaying)
            {
                Pause();
                Seek(0);
            }

            _isPlaying = true;
            _player.Play();
        }

        ///<Summary>
        /// Pause playback if playing (does not resume)
        ///</Summary>
        public void Pause()
        {
            _isPlaying = false;
            _player?.Pause();
        }

        ///<Summary>
        /// Stop playack and set the current position to the beginning
        ///</Summary>
        public void Stop()
        {
            Pause();
            Seek(0);
        }

        ///<Summary>
        /// Seek a position in seconds in the currently loaded sound file 
        ///</Summary>
        public void Seek(double position)
        {
            if (_player == null) return;
            _player.Position = TimeSpan.FromSeconds(position);
        }

        private void SetVolume(double volume, double balance)
        {
            if (_player == null || _isDisposed) return;

            _player.Volume = Math.Min(1, Math.Max(0, volume));
            _player.Balance = Math.Min(1, Math.Max(-1, balance));
        }

        private static MediaPlayer GetPlayer()
        {
            return new MediaPlayer();
        }

        private bool _isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed || _player == null)
                return;

            if (disposing)
                DeletePlayer();

            _isDisposed = true;
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
