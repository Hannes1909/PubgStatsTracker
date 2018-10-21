using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;

namespace GetPubgStats
{
    class Program
    {
        static void Main(string[] args)
        {
            var pubgStatsHTTP = new PubgStatsHTTP();

            string apikey = PubgStatsHelper.GetRandomApiKey();
            string accountid = "account.2b1f61ca3b3448b1b9b06aa4ab2338c6";
            string seasonid = "division.bro.official.pc-2018-01";
            string baseUrl = "https://api.pubg.com/shards/pc-eu/players?filter[playerNames]=Hannes1909";

            HttpClientHandler clientHandler = pubgStatsHTTP.ClientHandler;
            HttpRequestMessage buildRequest = pubgStatsHTTP.BuildRequest(baseUrl, apikey);

            using (HttpClient client = new HttpClient(clientHandler))
            {
                var response = client.SendAsync(buildRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                var responseContent = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                var stats = JsonConvert.DeserializeObject<PlayerSearchResult>(responseContent);

                
                {
                    Console.WriteLine("Deserialized: " + stats.data[0].id);
                }
                
            }
            Console.ReadLine();
        }

        

    }

}