using AVFoundation;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Plugin.SimpleAudioRecorder
{
    public class SimpleAudioRecorderImplementation : ISimpleAudioRecorder
    {
        public bool CanRecordAudio => true;

        AVAudioRecorder recorder;

        public SimpleAudioRecorderImplementation()
        {
            InitAudioSession();
            InitAudioRecorder();
        }

        void InitAudioRecorder()
        {
            var url = NSUrl.FromFilename(GetTempFileName());

            var settings = NSDictionary.FromObjectsAndKeys(objects, keys);

            recorder = AVAudioRecorder.Create(url, new AudioSettings(settings), out NSError error);

            recorder.PrepareToRecord();
        }

        string GetTempFileName()
        {
            var docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libFolder = Path.Combine(docFolder, "..", "Library");
            var tempFileName = Path.Combine(libFolder, Path.GetTempFileName());

            return tempFileName;
        }

        void InitAudioSession()
        {
#if __IOS_
            var audioSession = AVAudioSession.SharedInstance();

            var err = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
            if (err != null) throw new Exception(err.ToString());

            err = audioSession.SetActive(true);
            if (err != null) throw new Exception(err.ToString());
#endif
        }

        public Task RecordAsync()
        {
            return Task.FromResult(recorder.Record());
        }

        public Task<AudioRecording> StopAsync()
        {
            recorder.Stop();

            var recording = new AudioRecording(recorder.Url.Path);

            return Task.FromResult(recording);
        }

        static NSObject[] keys = new NSObject[]
        {
            AVAudioSettings.AVSampleRateKey,
            AVAudioSettings.AVFormatIDKey,
            AVAudioSettings.AVNumberOfChannelsKey,
            AVAudioSettings.AVLinearPCMBitDepthKey,
            AVAudioSettings.AVLinearPCMIsBigEndianKey,
            AVAudioSettings.AVLinearPCMIsFloatKey
        };

        static NSObject[] objects = new NSObject[]
        {
            NSNumber.FromFloat (16000), //Sample Rate
            NSNumber.FromInt32 ((int)AudioToolbox.AudioFormatType.LinearPCM), //AVFormat
            NSNumber.FromInt32 (1), //Channels
            NSNumber.FromInt32 (16), //PCMBitDepth
            NSNumber.FromBoolean (false), //IsBigEndianKey
            NSNumber.FromBoolean (false) //IsFloatKey
        };
    }
}