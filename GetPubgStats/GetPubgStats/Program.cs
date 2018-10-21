using System;
using System.Net.Http;

namespace GetPubgStats
{
    class Program
    {
        static void Main(string[] args)
        {
            var pubgStatsHTTP = new PubgStatsHTTP();

            string apikey = PubgStatsHelper.GetRandomApiKey();
            string accountid = "account.2b1f61ca3b3448b1b9b06aa4ab2338c6";
            string seasonid = "division.bro.official.2018-09";
            string baseUrl = "https://api.pubg.com/shards/pc-eu/players/" + accountid + "/seasons/" + seasonid;

            HttpClientHandler clientHandler = pubgStatsHTTP.ClientHandler;
            HttpRequestMessage buildRequest = pubgStatsHTTP.BuildRequest(baseUrl, apikey);

            using (HttpClient client = new HttpClient(clientHandler))
            {
                var response = client.SendAsync(buildRequest).ConfigureAwait(false).GetAwaiter().GetResult();
                var responseContent = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                Console.WriteLine(responseContent);
            }
            Console.ReadLine();
        }

        

    }

}