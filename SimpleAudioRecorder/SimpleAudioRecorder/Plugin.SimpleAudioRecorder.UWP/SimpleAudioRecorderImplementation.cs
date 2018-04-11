using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SimpleAudioRecorder
{
    public class SimpleAudioRecorderImplementation : ISimpleAudioRecorder
    {
        public bool CanRecordAudio => true;

        public Task RecordAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AudioRecording> StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
