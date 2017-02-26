using System;
using System.IO;

namespace Plugin.SimpleAudioPlayer.Abstractions
{
  /// <summary>
  /// Interface for SimpleAudioPlayer
  /// </summary>
  public interface ISimpleAudioPlayer : IDisposable
  {
        double Duration { get; }
        double CurrentPosition { get; }
        double Volume { get; set; }

        double Balance { get; set; }

        bool IsPlaying { get; }

        bool CanSeek { get; }
        
        bool Load(Stream audioStream);

        bool Load(string fileName);

        void Play();

        void Pause();

        void Stop();

        void Seek(double position);
    }
}
