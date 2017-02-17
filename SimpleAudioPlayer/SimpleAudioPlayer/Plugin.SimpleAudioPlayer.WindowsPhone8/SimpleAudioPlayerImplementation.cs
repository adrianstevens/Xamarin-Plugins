using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace Plugin.SimpleAudioPlayer
{
  /// <summary>
  /// Implementation for SimpleAudioPlayer
  /// </summary>
  public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
  {
        MediaElement player;

        public double Duration
        { get { return player == null ? 0 : player.NaturalDuration.TimeSpan.TotalSeconds; } }

        public double CurrentPosition
        { get { return player == null ? 0 : player.Position.TotalSeconds; } }

        public double Volume
        {
            get { return player == null ? 0 : player.Volume; }
            set { SetVolume(value); }
        }

        public bool IsPlaying
        {
            get
            {
                if (player == null )
                    return false;
                return player.CurrentState == MediaElementState.Playing; //might need to expand
            }
        }

        public bool Load(Stream audioStream)
        {
            if (player == null)
            {
                player = new MediaElement() { AutoPlay = false };
            }

            player.SetSource(audioStream);

            return (player == null) ? false : true;
        }

        public void Play()
        {
            if (player == null)
                return;

            if (player.CurrentState == MediaElementState.Playing)
            {
                player.Stop();
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

        public void Seek (double position)
        {
            if (player != null && player.CanSeek)
                player.Position = TimeSpan.FromSeconds(position);
        }

        public void SetVolume(double volume)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            player.Volume = volume;
        }
    }
}