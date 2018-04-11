using System;
using System.IO;

namespace Plugin.SimpleAudioRecorder
{
    public class AudioRecording : IDisposable
    {
        public AudioRecording(string filePath)
        {
            this.filePath = filePath;
        }

        public bool HasRecording => File.Exists(filePath);

        string filePath;

        public string GetFilePath ()
        {
            return filePath;
        }

        public Stream GetAudioStream()
        {
            if(File.Exists(filePath))
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return null;
        }

        void DeleteFile ()
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            filePath = string.Empty;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DeleteFile();
            }
        }
    }
}