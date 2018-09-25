using System;
using System.IO;
using NAudio.Wave;

namespace Plugin.SimpleAudioPlayer
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public sealed class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
    {
        private readonly WaveOutEvent _waveOut;

        private WaveFileReader _reader;

        public SimpleAudioPlayerImplementation()
        {
            _waveOut = new WaveOutEvent();
            _waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (IsPlaying && Loop && CanSeek)
            {
                Seek(0);
                Play();
            }
            else
            {
                PlaybackEnded?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _isDisposed;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (_isDisposed) return;

            _waveOut.PlaybackStopped -= OnPlaybackStopped;
            _waveOut.Dispose();

            _reader?.Dispose();
            _reader = null;

            _isDisposed = true;
        }

        ///<Summary>
        /// Raised when audio playback completes successfully 
        ///</Summary>
        public event EventHandler PlaybackEnded;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration => _reader?.TotalTime.TotalSeconds ?? 0;

        ///<Summary>
        /// Current position of audio playback in seconds
        ///</Summary>
        public double CurrentPosition => _reader?.CurrentTime.TotalSeconds ?? 0;

        ///<Summary>
        /// Playback volume 0 to 1 where 0 is no-sound and 1 is full volume
        ///</Summary>
        public double Volume
        {
            get => _waveOut.Volume;
            set => _waveOut.Volume = (float)Math.Min(1, Math.Max(0, value));
        }

        ///<Summary>
        /// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
        ///</Summary>
        public double Balance
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        public bool IsPlaying { get; private set; }

        ///<Summary>
        /// Continuously repeats the currently playing sound
        ///</Summary>
        public bool Loop
        {
            get;
            set;
        }

        ///<Summary>
        /// Indicates if the position of the loaded audio file can be updated
        ///</Summary>
        public bool CanSeek => _reader?.CanSeek ?? false;

        ///<Summary>
        /// Load wav audio file as a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            _reader = new WaveFileReader(audioStream);
            _waveOut.Init(_reader);
            return true;
        }

        ///<Summary>
        /// Load wav audio file from local path
        ///</Summary>
        public bool Load(string fileName)
        {
            _reader = new WaveFileReader(fileName);
            _waveOut.Init(_reader);
            return true;
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
        public void Play()
        {
            IsPlaying = true;
            _waveOut.Play();
        }

        ///<Summary>
        /// Pause playback if playing (does not resume)
        ///</Summary>
        public void Pause()
        {
            IsPlaying = false;
            _waveOut.Pause();
        }

        ///<Summary>
        /// Stop playback and set the current position to the beginning
        ///</Summary>
        public void Stop()
        {
            IsPlaying = false;
            _waveOut.Stop();
        }

        ///<Summary>
        /// Set the current playback position (in seconds)
        ///</Summary>
        public void Seek(double position)
        {
            if (_reader == null) return;
            _reader.CurrentTime = TimeSpan.FromSeconds(position);
        }
    }
}
