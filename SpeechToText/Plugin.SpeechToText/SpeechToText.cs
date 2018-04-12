using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Plugin.SpeechToText
{
    public class SpeechToText
    {
        AuthenticationService authenticationService;

        HttpClient httpClient;

        public SpeechToText(string bingSpeechApiKey)
        {
            authenticationService = new AuthenticationService(bingSpeechApiKey);
        }

        public async Task<SpeechToTextResult> RecognizeSpeechAsync(string audioFile)
        {
            SpeechToTextResult result;

            using (FileStream stream = File.OpenRead(audioFile))
            {
                var requestUri = GetRequestUri(Constants.SpeechRecognitionEndpoint);
                var accessToken = await authenticationService.GetAccessToken();

                var response = await SendRequestAsync(stream, requestUri, accessToken, Constants.AudioContentType);

                result = JsonConvert.DeserializeObject<SpeechToTextResult>(response);
            }
            return result;
        }

        string GetRequestUri(string speechEndpoint)
        {
            return $"{speechEndpoint}dictation/cognitiveservices/v1?language=en-us&format=simple";
        }

        async Task<string> SendRequestAsync(Stream fileStream, string url, string bearerToken, string contentType)
        {
            if (httpClient == null)
                httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var content = new StreamContent(fileStream);
            content.Headers.TryAddWithoutValidation("Content-Type", contentType);

            var response = await httpClient.PostAsync(url, content);

            return await response.Content.ReadAsStringAsync();
        }
    }
}