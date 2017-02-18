using Android.Content.Res;
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

        ///<Summary>
        /// Load wave or mp3 audio file as a stream
        ///</Summary>
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
            player?.SetDataSource(path);
            player?.Prepare();

            return (player == null) ? false : true;
        }

        ///<Summary>
        /// Load wave or mp3 audio file from the iOS Resources folder
        ///</Summary>
        public bool Load(string fileName)
        {
            AssetFileDescriptor afd = Android.App.Application.Context.Assets.OpenFd(fileName);

            player?.Dispose();
            player = new Android.Media.MediaPlayer();

            player?.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);
            
            player?.Prepare();

            return (player == null) ? false : true;
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
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

        ///<Summary>
        /// Stop playack and set the current position to the beginning
        ///</Summary>
        public void Stop()
        {
            player?.Pause();
            player?.SeekTo(0);
        }

        ///<Summary>
        /// Pause playback if playing (does not resume)
        ///</Summary>
        public void Pause()
        {
            player?.Pause();
        }

        ///<Summary>
        /// Sets the playback volume as a double between 0 and 1
        /// Sets both left and right channels
        ///</Summary>
        public void Seek (double position)
        {
            if (player != null)
                player.SeekTo((int)position*1000);
        }

        void SetVolume(double volume)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            player.SetVolume((float)volume, (float)volume);
        }
    }
}