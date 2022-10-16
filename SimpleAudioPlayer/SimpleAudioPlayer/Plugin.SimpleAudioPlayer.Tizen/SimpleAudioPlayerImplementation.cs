﻿using System;
using System.IO;
using System.Threading.Tasks;
using Tizen.Applications;
using Tizen.Multimedia;

namespace Plugin.SimpleAudioPlayer
{
	/// <summary>
	/// Implementation for Feature
	/// </summary>
	public class SimpleAudioPlayerImplementation : ISimpleAudioPlayer
	{
		///<Summary>
		/// Raised when audio playback completes successfully 
		///</Summary>
		public event EventHandler PlaybackEnded;

		private long previousSeekTime = -1L;
		private int lastRequestedSeekPosition;

		Player player;

		///<Summary>
		/// Length of audio in seconds
		///</Summary>
		public double Duration => (player?.StreamInfo?.GetDuration() / 1000.0) ?? 0;


        ///<Summary>
        /// Current position of audio playback in seconds
        ///</Summary>
        public double CurrentPosition => (player?.GetPlayPosition() / 1000.0) ?? 0;

        ///<Summary>
        /// Playback volume (0 to 1)
        ///</Summary>
        public double Volume 
		{
			get => player?.Volume ?? 0;
			set => SetVolume(value, Balance);
		}

		///<Summary>
		/// Balance left/right: -1 is 100% left : 0% right, 1 is 100% right : 0% left, 0 is equal volume left/right
		///</Summary>
		public double Balance
		{
            get => _balance;
			set => SetVolume(Volume, _balance = value);
		}
		double _balance = 0;

		///<Summary>
		/// Indicates if the currently loaded audio file is playing
		///</Summary>
		public bool IsPlaying => player == null ? false : player.State == PlayerState.Playing;

		///<Summary>
		/// Continously repeats the currently playing sound
		///</Summary>
		public bool Loop
		{
            get => _loop;
			set 
			{
				_loop = value;
				if (player != null)
                {
                    player.IsLooping = _loop;
                }
            }
		}
		bool _loop;

		///<Summary>
		/// Indicates if the position of the loaded audio file can be updated
		///</Summary>
		public bool CanSeek => player != null;

		

		///<Summary>
		/// Load wav or mp3 audio file as a stream
		///</Summary>
		public bool Load(Stream audioStream)
		{
			try
			{
				DeletePlayer();

    			player = new Player();
				
				if (player != null)
				{
					if (player.State != PlayerState.Idle)
						player.Unprepare();

					player.SetSource(new MediaBufferSource(ReadBuffer(audioStream), (int)audioStream.Length));
					PreparePlayer();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
			return (player != null);
		}

		///<Summary>
		/// Load wav or mp3 audio file from the iOS Resources folder
		///</Summary>
		public bool Load(string fileName)
		{
			try
			{
				DeletePlayer();

				player = new Player();

				if (player != null)
				{
					if (player.State != PlayerState.Idle)
					{
						player.Unprepare();
					}

                    var audioPath = Path.Combine(Application.Current.DirectoryInfo.Resource, fileName);

                    player.SetSource(new MediaUriSource(audioPath));
					PreparePlayer();
				}
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

			return (player != null);
		}

		async void PreparePlayer()
		{
			player.PlaybackCompleted += OnPlaybackEnded;
			lastRequestedSeekPosition = -1;
			await player.PrepareAsync();
		}

		///<Summary>
		/// Pause playback if playing (does not resume)
		///</Summary>
		public void Pause()
		{
			try
			{
				if (player != null)
				{
					if (player.State == PlayerState.Playing)
						player.Pause();
				}
				else
					return;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		///<Summary>
		/// Stop playack and set the current position to the beginning
		///</Summary>
		public async void Stop()
		{
			try
			{
				if (player != null)
				{
					if (player.State == PlayerState.Playing || player.State == PlayerState.Paused)
					{
						player.Stop();
						await SeekToAsync((int)CurrentPosition);
						PlaybackEnded?.Invoke(this, EventArgs.Empty);
					}
				}
				else
					return;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		///<Summary>
		/// Begin playback or resume if paused
		///</Summary>
		public async void Play()
		{
			try
			{
				if (player != null)
				{
					if (player.State == PlayerState.Playing)
					{
						player.Pause();
						await SeekToAsync((int)CurrentPosition);
						player.Start();
					}
					else if (player.State == PlayerState.Ready)
					{
						await SeekToAsync((int)CurrentPosition);
						player.Start();
					}
					else
					{
						player.Start();
					}
				}
				else
					return;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

        public void SetSpeed(double speed)
        {
			player.SetPlaybackRate((float)speed);
        }

        ///<Summary>
        /// Set the current playback position (in seconds)
        ///</Summary>
        public void Seek(double position)
		{
			throw new PlatformNotSupportedException();
		}

		///<Summary>
		/// Set the current playback position (in seconds)
		///</Summary>
		public async Task SeekToAsync(int msec)
		{
			try
			{
				if (player != null)
				{
					if (player.State == PlayerState.Ready || player.State == PlayerState.Playing || player.State == PlayerState.Paused)
					{
						if (lastRequestedSeekPosition == msec || player == null)
							return;

						var nowTicks = DateTime.Now.Ticks;
						lastRequestedSeekPosition = msec;

						if (previousSeekTime == -1L)
							previousSeekTime = nowTicks;

						var diffInMilliseconds = (nowTicks - previousSeekTime) / TimeSpan.TicksPerMillisecond;
						if (diffInMilliseconds < 1000)
							await Task.Delay(TimeSpan.FromMilliseconds(2000));
						if (player == null)
							return;

						previousSeekTime = nowTicks;
						if (lastRequestedSeekPosition != msec)
							return;

						await player.SetPlayPositionAsync(lastRequestedSeekPosition, false);
						lastRequestedSeekPosition = -1;
					}
				}
				else
					return;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		///<Summary>
		/// Sets the playback volume as a double between 0 and 1
		/// Sets both left and right channels
		///</Summary>
		void SetVolume(double volume, double balance)
		{
			if (player == null || isDisposed)
				return;

			volume = Math.Max(0, volume);
			volume = Math.Min(1, volume);

			//balance = Math.Max(-1, balance);
			//balance = Math.Min(1, balance);

			player.Volume = (float)volume;
        }

		void DeletePlayer()
		{
			try
			{
				Stop();

				if (player != null)
				{
					if (player.State == PlayerState.Ready || player.State == PlayerState.Playing || player.State == PlayerState.Paused)
					{
						player.PlaybackCompleted -= OnPlaybackEnded;
						player.Unprepare();
						player.Dispose();
						player = null;
					}
				}
				else
					return;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		void OnPlaybackEnded(object sender, EventArgs e)
		{
			PlaybackEnded?.Invoke(sender, e);
		}

        static byte[] ReadBuffer(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        bool isDisposed = false;

		///<Summary>
		/// Dispose SimpleAudioPlayer and release resources
		///</Summary>
		protected virtual void Dispose(bool disposing)
		{
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                DeletePlayer();
            }

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