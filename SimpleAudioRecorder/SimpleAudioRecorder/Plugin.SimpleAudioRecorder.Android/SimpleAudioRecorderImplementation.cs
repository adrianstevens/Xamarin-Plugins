﻿using System.IO;
using System.Threading.Tasks;
using Android.Media;
using Android.Content;
using System;
using Java.IO;
using Android.App;

namespace Plugin.SimpleAudioRecorder
{
    public class SimpleAudioRecorderImplementation : ISimpleAudioRecorder
    {
        public bool CanRecordAudio { get; private set; } = true;

        public bool IsRecording => audioRecord?.RecordingState == RecordState.Recording;

        AudioRecord audioRecord;

        string rawFilePath;
        string audioFilePath;

        int bufferSize;
        int sampleRate;

        public SimpleAudioRecorderImplementation ()
        {
            var pm = Application.Context.PackageManager;
            CanRecordAudio = pm.HasSystemFeature(Android.Content.PM.PackageManager.FeatureMicrophone);
        }

        AudioRecord GetAudioRecord(int sampleRate)
        {
            this.sampleRate = sampleRate;
            var channelConfig = ChannelIn.Mono;
            var encoding = Encoding.Pcm16bit;

            bufferSize = AudioRecord.GetMinBufferSize(sampleRate, channelConfig, encoding) * 8;

            return new AudioRecord(AudioSource.Mic, sampleRate, ChannelIn.Stereo, encoding, bufferSize);
        }

        public string GetAudioFilePath()
        {
            return audioFilePath;
        }

        AudioRecording GetRecording()
        {
            if (audioRecord == null ||
                audioRecord.RecordingState == RecordState.Recording ||
                System.IO.File.Exists(audioFilePath) == false)
                return null;

            return new AudioRecording(audioFilePath);
        }

        public Task RecordAsync()
        {
            if (CanRecordAudio == false || audioRecord?.RecordingState == RecordState.Recording)
                return Task.CompletedTask;

            var audioManager = (AudioManager)Application.Context.GetSystemService(Context.AudioService);

            var micSampleRate = Int32.Parse(audioManager.GetProperty(AudioManager.PropertyOutputSampleRate));

            audioRecord = GetAudioRecord(micSampleRate);

            audioRecord.StartRecording();

            return Task.Run(() => WriteAudioDataToFile());
        }

        string GetTempFileName()
        {
            return Path.Combine("/sdcard/", Path.GetTempFileName());
        }

        void WriteAudioDataToFile()
        {
            var data = new byte[bufferSize];

            rawFilePath = GetTempFileName();

            FileOutputStream outputStream = null;

            try
            {
                outputStream = new FileOutputStream(rawFilePath);
            }
            catch (Exception ex)
            {
                throw new FileLoadException($"unable to create a new file: {ex.Message}");
            }

            if (outputStream != null)
            {
                while (audioRecord.RecordingState == RecordState.Recording)
                {
                    audioRecord.Read(data, 0, bufferSize);

                    outputStream.Write(data);
                }

                outputStream.Close();
            }
        }

        public Task<AudioRecording> StopAsync()
        {
            if (audioRecord?.RecordingState == RecordState.Recording)
            {
                audioRecord?.Stop();
            }

            audioFilePath = GetTempFileName();

            CopyWaveFile(rawFilePath, audioFilePath);

            return Task.FromResult(GetRecording());
        }

        void CopyWaveFile(string sourcePath, string destinationPath)
        {
            FileInputStream inputStream = null;
            FileOutputStream outputStream = null;

            int channels = 2;
            long byteRate = 16 * sampleRate * channels / 8;

            var data = new byte[bufferSize];

            try
            {
                inputStream = new FileInputStream(sourcePath);
                outputStream = new FileOutputStream(destinationPath);
                var totalAudioLength = inputStream.Channel.Size();
                var totalDataLength = totalAudioLength + 36;

                WriteWaveFileHeader(outputStream, totalAudioLength, totalDataLength, sampleRate, channels, byteRate);

                while (inputStream.Read(data) != -1)
                {
                    outputStream.Write(data);
                }

                inputStream.Close();
                outputStream.Close();

            }
            catch { }
        }

        void WriteWaveFileHeader(FileOutputStream outputStream, long audioLength, long dataLength, long sampleRate, int channels, long byteRate)
        {
            byte[] header = new byte[44];

            header[0] = Convert.ToByte('R'); // RIFF/WAVE header
            header[1] = Convert.ToByte('I'); // (byte)'I'
            header[2] = Convert.ToByte('F');
            header[3] = Convert.ToByte('F');
            header[4] = (byte)(dataLength & 0xff);
            header[5] = (byte)((dataLength >> 8) & 0xff);
            header[6] = (byte)((dataLength >> 16) & 0xff);
            header[7] = (byte)((dataLength >> 24) & 0xff);
            header[8] = Convert.ToByte('W');
            header[9] = Convert.ToByte('A');
            header[10] = Convert.ToByte('V');
            header[11] = Convert.ToByte('E');
            header[12] = Convert.ToByte('f'); // fmt chunk
            header[13] = Convert.ToByte('m');
            header[14] = Convert.ToByte('t');
            header[15] = (byte)' ';
            header[16] = 16; // 4 bytes - size of fmt chunk
            header[17] = 0;
            header[18] = 0;
            header[19] = 0;
            header[20] = 1; // format = 1
            header[21] = 0;
            header[22] = Convert.ToByte(channels);
            header[23] = 0;
            header[24] = (byte)(sampleRate & 0xff);
            header[25] = (byte)((sampleRate >> 8) & 0xff);
            header[26] = (byte)((sampleRate >> 16) & 0xff);
            header[27] = (byte)((sampleRate >> 24) & 0xff);
            header[28] = (byte)(byteRate & 0xff);
            header[29] = (byte)((byteRate >> 8) & 0xff);
            header[30] = (byte)((byteRate >> 16) & 0xff);
            header[31] = (byte)((byteRate >> 24) & 0xff);
            header[32] = (byte)(2 * 16 / 8); // block align
            header[33] = 0;
            header[34] = Convert.ToByte(16); // bits per sample
            header[35] = 0;
            header[36] = Convert.ToByte('d');
            header[37] = Convert.ToByte('a');
            header[38] = Convert.ToByte('t');
            header[39] = Convert.ToByte('a');
            header[40] = (byte)(audioLength & 0xff);
            header[41] = (byte)((audioLength >> 8) & 0xff);
            header[42] = (byte)((audioLength >> 16) & 0xff);
            header[43] = (byte)((audioLength >> 24) & 0xff);

            outputStream.Write(header, 0, 44);
        }
    }
}