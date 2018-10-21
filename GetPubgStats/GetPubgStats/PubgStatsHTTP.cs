using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GetPubgStats
{
    class PubgStatsHTTP
    {
        public HttpClientHandler ClientHandler { get => clientHandler; }
        public HttpRequestMessage BuildRequest(string url, string apiToken) { return buildRequest(url, apiToken); }

        private readonly HttpClientHandler clientHandler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };

        private HttpRequestMessage buildRequest(string url, string apiToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
            request.Headers.Add("Accept-Encoding", "gzip");
            request.Headers.Add("Accept", "application/vnd.api+json");
            return request;
        }
    }
}
