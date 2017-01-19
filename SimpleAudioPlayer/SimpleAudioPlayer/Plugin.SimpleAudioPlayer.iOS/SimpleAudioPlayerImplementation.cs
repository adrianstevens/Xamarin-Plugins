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

        public void SetVolume(double volume)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            player.SetVolume((float)volume, (float)volume);
        }
    }
}