using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Plugin.SimpleAudioPlayer
{
  /// <summary>
  /// Implementation for Feature
  /// </summary>
  public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
  {
        MediaPlayer player;

        public double Duration
        { get { return player == null ? 0 : player.PlaybackSession.NaturalDuration.TotalSeconds; } }

        public double CurrentPosition
        { get { return player == null ? 0 : player.PlaybackSession.Position.TotalSeconds; } }

        public double Volume
        {
            get { return player == null ? 0 : player.Volume; }
            set { SetVolume(value); }
        }

        public bool IsPlaying
        {
            get
            {
                if (player == null || player.PlaybackSession == null)
                    return false;
                return player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing; //might need to expand
            }
        }

        public bool CanSeek
        { get { return player == null ? false : player.PlaybackSession.CanSeek; } }

        ///<Summary>
        /// Load wave or mp3 audio file from a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            player?.Dispose();
            player = new MediaPlayer() { AutoPlay = false };

            //player.SetStreamSource(audioStream.AsRandomAccessStream());

            player.Source = MediaSource.CreateFromStream(audioStream.AsRandomAccessStream(), string.Empty);

            return (player == null || player.Source == null) ? false : true;
        }

        ///<Summary>
        /// Load wave or mp3 audio file from assets folder in the UWP project
        ///</Summary>
        public bool Load(string fileName)
        {
            player?.Dispose();
            player = new MediaPlayer() { AutoPlay = false };

            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/" + fileName));
 
            return (player == null || player.Source == null) ? false : true;
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
        public void Play()
        {
            if (player == null)
                return;

            if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                Pause();
                Seek(0);
                player.Play();
            }
            else
            {
                player.Play();
            }
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
            if (player != null)
            {
                Pause();
                Seek(0);
            }
        }

        ///<Summary>
        /// Seek a position in seconds in the currently loaded sound file 
        ///</Summary>
        public void Seek (double position)
        {
            if (player == null || player.PlaybackSession == null)
                return;

            if (player.PlaybackSession.CanSeek)
                player.PlaybackSession.Position = TimeSpan.FromSeconds(position);
        }

        public void SetVolume(double volume)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            player.Volume = volume;
        }
    }
}