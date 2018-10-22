using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PubgAPI
{
    /// <summary>
    /// execute the APICalls and deserialize the json-objs in strongly typed objects
    /// </summary>
    class PubgAPICalls
    {
        string[] apiKeys;

        public PubgAPICalls()
        {
        }

        public void SetAPIKeys(string[] Keys)
        {
            this.apiKeys = Keys;
        }

        /// <summary>
        /// search for the player data
        /// 
        /// only 1 match is valid, no multimatches
        /// </summary>
        /// <param name="Playername"></param>
        /// <returns></returns>
        public PubgAPI.Player GetPlayerData( string Playername)
        {
            var responseContent = this.HttpRequest($"https://api.pubg.com/shards/pc-eu/players?filter[playerNames]={Playername}");
            var queryresult = JsonConvert.DeserializeObject<PlayerSearchResult>(responseContent.Result.ReadAsStringAsync().Result);
            if (queryresult?.data?.Count != 1)
            {
                throw new Exception("Player not found");
            }

            return queryresult.data[0];
        }

        /// <summary>
        /// construct http-request with apikeys
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<HttpContent> HttpRequest(string url)
        {
            if (this.apiKeys.Length == 0)
            {
                throw new Exception("No APIKeys loaded");
            }

            HttpClient client = new HttpClient(new HttpClientHandler
                                                        {
                                                            AutomaticDecompression = System.Net.DecompressionMethods.GZip
                                                        });
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.AcceptEncoding.Clear();
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.api+json");

            HttpResponseMessage _httpresult = null;

            Random _random = new Random();
            foreach (string apikey in this.apiKeys.OrderBy( _a => _random.Next() ))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

                _httpresult = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));
                if (_httpresult.IsSuccessStatusCode)
                {
                    return _httpresult.Content;
                }

                // no ratelimit problem ==> break
                if (_httpresult.StatusCode != System.Net.HttpStatusCode.TooManyRequests)
                {
                    break;
                }
            }

            if (_httpresult == null)
            {
                throw new Exception("No HTTP-Request executed");
            }
            throw new Exception("Last Response: " + _httpresult.ToString());
        }
    }
}
