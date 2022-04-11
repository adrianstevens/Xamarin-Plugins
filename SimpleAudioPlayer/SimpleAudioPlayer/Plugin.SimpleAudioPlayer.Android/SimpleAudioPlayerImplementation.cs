using Android.Content.Res;
using System;
using System.IO;
using Uri = Android.Net.Uri;

namespace Plugin.SimpleAudioPlayer
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    [Android.Runtime.Preserve(AllMembers = true)]
    public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
    {
        ///<Summary>
        /// Raised when audio playback completes successfully 
        ///</Summary>
        public event EventHandler PlaybackEnded;

        Android.Media.MediaPlayer player;

        static int index = 0;

        ///<Summary>
        /// Length of audio in seconds
        ///</Summary>
        public double Duration => player == null ? 0 : player.Duration / 1000.0; 

        ///<Summary>
        /// Current position of audio playback in seconds
        ///</Summary>
        public double CurrentPosition => player == null ? 0 : (player.CurrentPosition) / 1000.0; 

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume
        {
            get =>_volume; 
            set 
            {
                SetVolume(_volume = value, Balance); 
            }
        }
        double _volume = 0.5;

        ///<Summary>
        /// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
        ///</Summary>
        public double Balance
        {
            get => _balance;
            set 
            { 
                SetVolume(Volume, _balance = value); 
            }
        }
        double _balance = 0;

        ///<Summary>
        /// Indicates if the currently loaded audio file is playing
        ///</Summary>
        public bool IsPlaying => player != null && player.IsPlaying; 

        ///<Summary>
        /// Continously repeats the currently playing sound
        ///</Summary>
        public bool Loop
        {
            get => _loop; 
            set { _loop = value;  if (player != null) player.Looping = _loop; }
        }
        bool _loop;

        ///<Summary>
        /// Indicates if the position of the loaded audio file can be updated
        ///</Summary>
        public bool CanSeek => player != null;

        string path;

        /// <summary>
        /// Instantiates a new SimpleAudioPlayer
        /// </summary>
        public SimpleAudioPlayerImplementation()
        {
            player = new Android.Media.MediaPlayer() { Looping = Loop };
            player.Completion += OnPlaybackEnded;
        }

        ///<Summary>
        /// Load wav or mp3 audio file as a stream
        ///</Summary>
        public bool Load(Stream audioStream)
        {
            player.Reset();

            DeleteFile(path);

            //cache to the file system
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"cache{index++}.wav");
            var fileStream = File.Create(path);
            audioStream.CopyTo(fileStream);
            fileStream.Close();

            try
            {
                player.SetDataSource(path);
            }
            catch
            {
                try
                {
                    var context = Android.App.Application.Context;
                    player?.SetDataSource(context, Uri.Parse(Uri.Encode(path)));
                }
                catch
                {
                    return false;
                }
            }

            return PreparePlayer();
        }

        ///<Summary>
        /// Load wav or mp3 audio file from the iOS Resources folder
        ///</Summary>
        public bool Load(string fileName)
        {
            player.Reset();

            AssetFileDescriptor afd = Android.App.Application.Context.Assets.OpenFd(fileName);

            player?.SetDataSource(afd.FileDescriptor, afd.StartOffset, afd.Length);

            return PreparePlayer();
        }

        bool PreparePlayer()
        {
            player?.Prepare();

            return player != null;
        }

        void DeletePlayer()
        {
            Stop();

            if (player != null)
            {
                player.Completion -= OnPlaybackEnded;
                player.Release();
                player.Dispose();
                player = null;
            }

            DeleteFile(path);
            path = string.Empty;
        }

        void DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) == false)
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                }
            }
        }

        ///<Summary>
        /// Begin playback or resume if paused
        ///</Summary>
        public void Play()
        {
            if (player == null)
                return;

            if (IsPlaying)
            {
                Pause();
                Seek(0);
            }

            player.Start();
        }

        ///<Summary>
        /// Stop playack and set the current position to the beginning
        ///</Summary>
        public void Stop()
        {
	        if(!IsPlaying)
	    	    return;
		
            Pause();
            Seek(0);
            PlaybackEnded?.Invoke(this, EventArgs.Empty);
        }

        ///<Summary>
        /// Pause playback if playing (does not resume)
        ///</Summary>
        public void Pause()
        {
            player?.Pause();
        }

        ///<Summary>
        /// Set the current playback position (in seconds)
        ///</Summary>
        public void Seek(double position)
        {
	    if(CanSeek)
            	player?.SeekTo((int)(position * 1000D));
        }

        ///<Summary>
        /// Sets the playback volume as a double between 0 and 1
        /// Sets both left and right channels
        ///</Summary>
        void SetVolume(double volume, double balance)
        {
            volume = Math.Max(0, volume);
            volume = Math.Min(1, volume);

            balance = Math.Max(-1, balance);
            balance = Math.Min(1, balance);

            // Using the "constant power pan rule." See: http://www.rs-met.com/documents/tutorials/PanRules.pdf
            var left = Math.Cos((Math.PI * (balance + 1)) / 4) * volume;
            var right = Math.Sin((Math.PI * (balance + 1)) / 4) * volume;

            player?.SetVolume((float)left, (float)right);
        }

        void OnPlaybackEnded(object sender, EventArgs e)
        {
            PlaybackEnded?.Invoke(sender, e);

            //this improves stability on older devices but has minor performance impact
            // We need to check whether the player is null or not as the user might have dipsosed it in an event handler to PlaybackEnded above.
            if (player != null && Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
            {
                player.SeekTo(0);
                player.Stop();
                player.Prepare();
            }
        }

        bool isDisposed = false;

        ///<Summary>
		/// Dispose SimpleAudioPlayer and release resources
		///</Summary>
       	protected virtual void Dispose(bool disposing)
        {
            if (isDisposed || player == null)
                return;

            if (disposing)
                DeletePlayer();

            isDisposed = true;
        }

        ~SimpleAudioPlayerImplementation()
        {
            Dispose(false);
        }

		///<Summary>
		/// Dispose SimpleAudioPlayer and release resources
		///</Summary>
		public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
