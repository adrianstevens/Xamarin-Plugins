using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpeechToText
{
    internal class AuthenticationService
    {
        HttpClient httpClient;

        string accessToken;

        const int TokenLifeTime = 9;

        DateTime tokenTimeStamp = DateTime.MinValue;

        public AuthenticationService(string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

        public async Task<string> GetAccessToken ()
        {
            if(DateTime.Now - tokenTimeStamp > TimeSpan.FromSeconds(TokenLifeTime))
            {
                accessToken = await FetchTokenAsync();
                tokenTimeStamp = DateTime.Now;
            }

            return accessToken;
        }

        async Task<string> FetchTokenAsync()
        {
            var uriBuilder = new UriBuilder(Constants.AuthenticationTokenEndpoint);

            var result = await httpClient.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
            return await result.Content.ReadAsStringAsync();
        }
    }
}