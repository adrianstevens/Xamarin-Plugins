using System;
using System.IO;

namespace Plugin.SimpleAudioPlayer.Abstractions
{
  /// <summary>
  /// Interface for SimpleAudioPlayer
  /// </summary>
  public interface ISimpleAudioPlayer
  {
        bool Load(Stream audioStream);

        void Play();

        void Pause();

        void Stop();
    }
}
