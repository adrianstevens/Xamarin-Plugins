using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;
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
        { get { return player == null ? 0 : player.PlaybackSession.Position.TotalSeconds; } }

        public double CurrentPosition
        { get { return player == null ? 0 : player.PlaybackSession.NaturalDuration.TotalSeconds; } }

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

        public bool Load(Stream audioStream)
        {
            if (player == null)
            {
                player = new MediaPlayer() { AutoPlay = false };
            }

            //TODO
            player.SetStreamSource(audioStream.AsRandomAccessStream());

            return (player == null) ? false : true;
        }

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

        public void Pause()
        {
            player?.Pause();
        }

        public void Stop()
        {
            if (player != null)
            {
                Pause();
                Seek(0);
            }
        }

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