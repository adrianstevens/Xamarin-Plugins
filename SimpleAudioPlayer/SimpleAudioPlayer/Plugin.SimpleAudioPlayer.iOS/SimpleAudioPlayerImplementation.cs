using AVFoundation;
using Foundation;
using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;

namespace Plugin.SimpleAudioPlayer
{
  /// <summary>
  /// Implementation for SimpleAudioPlayer
  /// </summary>
  public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
  {
        AVAudioPlayer player;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration
        { get { return player == null ? 0 : player.Duration; } }

        ///<Summary>
        /// Current position of audio in seconds
        ///</Summary>
        public double CurrentPosition
        { get { return player == null ? 0 : player.CurrentTime; } }

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume
        {
            get { return player == null ? 0 : player.Volume; }
            set { SetVolume(value); }
        }

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        public bool IsPlaying
        { get { return player == null ? false : player.Playing; } }

        ///<Summary>
        /// Indicates if the currently loaded audio file can be seeked - always returns true on iOS
        ///</Summary>
        public bool CanSeek
        { get { return player == null ? false : true; } }

        ///<Summary>
        /// Load wave or mp3 audio file as a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            var data = NSData.FromStream(audioStream);

            Stop();
            player?.Dispose();

            player = AVAudioPlayer.FromData(data);

            return (player == null) ? false : true;
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

        ///<Summary>
        /// Sets the playback volume as a double between 0 and 1
        /// Sets both left and right channels
        ///</Summary>
        public void SetVolume(double volume)
        {
            if (player == null)
                return;

            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            player.SetVolume((float)volume, (float)volume);
        }
    }
}