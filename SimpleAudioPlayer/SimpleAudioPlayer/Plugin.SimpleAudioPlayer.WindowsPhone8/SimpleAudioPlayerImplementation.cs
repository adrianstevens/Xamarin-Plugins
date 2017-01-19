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
        MediaElement element;

        public bool Load(Stream audioStream)
        {
            if (element == null)
            {
                element = new MediaElement() { AutoPlay = false };
            }

            element.SetSource(audioStream);

            return (element == null) ? false : true;
        }

        public void Play()
        {
            if (element == null)
                return;

            if (element.CurrentState == MediaElementState.Playing)
            {
                element.Stop();
            }
            else
            {
                element.Play();
            }
        }

        public void Pause()
        {
            element?.Pause();
        }
        public void Stop()
        {
            if (element != null)
            {
                element.Stop();
                element.Position = TimeSpan.Zero;
            }
        }

        public void SetVolume(double volume)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            element.Volume = volume;
        }
    }
}