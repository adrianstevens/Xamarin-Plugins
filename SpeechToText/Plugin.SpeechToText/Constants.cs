namespace Plugin.SpeechToText
{
    internal static class Constants
    {
        public const string AuthenticationTokenEndpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

        public const string SpeechRecognitionEndpoint = "https://speech.platform.bing.com/speech/recognition/";
        public const string AudioContentType = @"audio/wav; codec=""audio/pcm""; samplerate=16000";
    }
}