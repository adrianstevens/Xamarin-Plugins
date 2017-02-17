using Plugin.SimpleAudioPlayer.Abstractions;
using System;
using System.IO;


namespace Plugin.SimpleAudioPlayer
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
    {
        Android.Media.MediaPlayer player;

        static int index = 0;

        public double Duration
        { get { return player == null ? 0 : ((double)player.Duration)/1000.0; } }

        public double CurrentPosition
        { get { return player == null ? 0 : ((double)player.CurrentPosition)/1000.0; } }

        public double Volume
        {
            get { return _volume; }
            set { SetVolume(_volume = value); }
        }
        double _volume = 0.5;

        public bool IsPlaying
        { get { return player == null ? false : player.IsPlaying; } }

        public bool CanSeek
        { get { return player == null ? false : true; } }

        public bool Load(Stream audioStream)
        {
            //cache to the file system
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"cache{index++}.wav");
            var fileStream = File.Create(path);
            audioStream.CopyTo(fileStream);
            fileStream.Close();

            //load the cached audio into MediaPlayer
            player?.Dispose();
            player = new Android.Media.MediaPlayer();
            player.SetDataSource(path);
            player.Prepare();

            return true;
        }

        public void Play()
        {
            if (player == null)
                return;

            if (player.IsPlaying)
            {
                player.Pause();
                player.SeekTo(0);
            }

            player.Start();
        }

        public void Stop()
        {
            player?.Stop();
            player?.Prepare();
            player?.SeekTo(0);
        }

        public void Pause()
        {
            player?.Pause();
        }

        public void Seek (double position)
        {
            if (player != null)
                player.SeekTo((int)position);
        }

        void SetVolume(double volume)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            player.SetVolume((float)volume, (float)volume);
        }
    }
}