using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;
using Windows.UI.Xaml.Controls;

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

            element.SetSource(audioStream.AsRandomAccessStream(), "");

            return (element == null) ? false : true;
        }

        public void Play()
        {
            if (element == null)
                return;

            if (element.CurrentState == global::Windows.UI.Xaml.Media.MediaElementState.Playing)
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