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

        public double Duration
        { get { return player == null ? 0 : player.Duration; } }

        public double CurrentPosition
        { get { return player == null ? 0 : player.CurrentTime; } }

        public double Volume
        {
            get { return player == null ? 0 : player.Volume; }
            set { SetVolume(value); }
        }

        public bool IsPlaying
        { get { return player == null ? false : player.Playing; } }

        public bool Load(Stream audioStream)
        {
            var data = NSData.FromStream(audioStream);

            Stop();
            player?.Dispose();

            player = AVAudioPlayer.FromData(data);

            return (player == null) ? false : true;
        }

        public void Play()
        {
            if (player == null)
                return;

            if (player.Playing)
                player.CurrentTime = 0;
            else
                player?.Play();
        }

        public void Pause()
        {
            player?.Pause();
        }

        public void Stop()
        {
            player?.Stop();
        }

        public void Seek (double position)
        {
            if (player == null)
                return;
            player.CurrentTime = position;
        }

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