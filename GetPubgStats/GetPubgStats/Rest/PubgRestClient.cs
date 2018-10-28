using GetPubgStats.Rest.RateLimiting;
using GetPubgStats.Rest.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using RestSharp;
using System;

namespace GetPubgStats.Rest
{
    public class PubgRestClient
    {
        private readonly RateLimiter rateLimiter;
        private readonly RestClient client;

        public PubgRestClient(string apiBaseUrl, string[] apiAccessTokens)
        {
            this.rateLimiter = new RateLimiter(apiAccessTokens);
            this.client = new RestClient(apiBaseUrl);
        }

        /// <summary>
        /// Queries the Pubg-WebApi for the player with the given username
        /// </summary>
        public PlayersQueryResult GetPlayerByName(string username)
        {
            return this.GetPlayersByName(new string[] { username });
        }

        /// <summary>
        /// Queries the Pubg-WebApi for the players with the given usernames. Up to six usernames can be queried
        /// </summary>
        public PlayersQueryResult GetPlayersByName(string[] usernames)
        {
            RestRequest request = this.CreateRequest("players");
            request.AddQueryParameter("filter[playerNames]", String.Join(",", usernames.Take(6)));

            IRestResponse response = this.rateLimiter.ExecuteRateLimitedRequest(this.client, request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestRequestException(response.StatusCode, response.Content);
            }

            return JsonConvert.DeserializeObject<PlayersQueryResult>(response.Content);
        }

        /// <summary>
        /// Queries the Pubg-WebApi for the player with the given id
        /// </summary>
        public PlayerQueryResult GetPlayerById(string id)
        {
            RestRequest request = this.CreateRequest("players/" + id);

            IRestResponse response = this.rateLimiter.ExecuteRateLimitedRequest(this.client, request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestRequestException(response.StatusCode, response.Content);
            }

            return JsonConvert.DeserializeObject<PlayerQueryResult>(response.Content);
        }

        /// <summary>
        /// Queries the Pubg-WebApi for the players with the given ids. Up to six user ids can be queried
        /// </summary>
        public PlayersQueryResult GetPlayersById(string[] ids)
        {
            RestRequest request = this.CreateRequest("players");
            request.AddQueryParameter("filter[playerIds]", String.Join(",", ids.Take(6)));

            IRestResponse response = this.rateLimiter.ExecuteRateLimitedRequest(this.client, request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestRequestException(response.StatusCode, response.Content);
            }

            return JsonConvert.DeserializeObject<PlayersQueryResult>(response.Content);
        }

        /// <summary>
        /// Queries the Pubg-WebApi for the <see cref="MatchQueryResult"/> with the given Id
        /// </summary>
        public MatchQueryResult GetMatch(string matchId)
        {
            RestRequest request = this.CreateRequest($"matches/{matchId}");

            IRestResponse response = this.client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestRequestException(response.StatusCode, response.Content);
            }

            return JsonConvert.DeserializeObject<MatchQueryResult>(response.Content);
        }

        private RestRequest CreateRequest(string resource)
        {
            RestRequest request = new RestRequest(resource, Method.GET, DataFormat.Json);
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddDecompressionMethod(DecompressionMethods.GZip);

            return request;
        }
    }
}
