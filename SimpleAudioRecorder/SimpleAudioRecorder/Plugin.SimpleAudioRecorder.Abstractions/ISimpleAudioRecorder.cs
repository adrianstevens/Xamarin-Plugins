using System.Threading.Tasks;

namespace Plugin.SimpleAudioRecorder
{
    /// <summary>
    /// Interface for SimpleAudioPlayer
    /// </summary>
    public interface ISimpleAudioRecorder
    {
        ///<Summary>
        /// Check if the executing device is capable of recording audio
        ///</Summary>
        bool CanRecordAudio { get; }

        ///<Summary>
        /// Start recording 
        ///</Summary>
        Task RecordAsync();

        ///<Summary>
        /// Stop recording and return the AudioRecording instance with the recording data
        ///</Summary>
        Task<AudioRecording> StopAsync();
    }
}
