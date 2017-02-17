using System;
using System.IO;

namespace Plugin.SimpleAudioPlayer.Abstractions
{
  /// <summary>
  /// Interface for SimpleAudioPlayer
  /// </summary>
  public interface ISimpleAudioPlayer
  {
        double Duration { get; }
        double CurrentPosition { get; }
        double Volume { get; set; }

        bool IsPlaying { get; }

        bool CanSeek { get; }


        bool Load(Stream audioStream);

        void Play();

        void Pause();

        void Stop();

        void Seek(double position);
    }
}
