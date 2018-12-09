using PubgStatsController.Rest.RateLimiting;
using PubgStatsController.Rest.Exceptions;
using PubgStatsController.Rest.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using RestSharp;
using System;

namespace PubgStatsController.Rest
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
        /// Queries the PUBG-web-api for the player with the given account id
        /// </summary>
        /// <param name="accountId">Id of the players to be searched for</param>
        /// <returns>Player that matches the given account id</returns>
        public Player GetPlayerByPlayerId(SelectorAccountId accountId)
        {
            RestRequest request = this.CreateRequest($"players/{accountId.Key}", null);

            IRestResponse response = this.rateLimiter.ExecuteRateLimitedRequest(this.client, request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestResponseException(response);
            }

            return JsonConvert.DeserializeObject<Player>(response.Content);
        }

        /// <summary>
        /// Queries the PUBG-web-api for the player with the given ingame name
        /// </summary>
        /// <param name="accountId">Ingame name of the player to be searched for</param>
        /// <returns>Player that matches the given ingame name</returns>
        public Player GetPlayerByIngameName(string ingameName)
        {
            List<Player> players = this.GetPlayersByIngameNames(new string[] { ingameName });

            if(players.Count < 1)
            {
                throw new Exception($"The player with the ingame name {ingameName} could not be found by the web-api.");
            }

            return players.First();
        }

        /// <summary>
        /// Queries the PUBG-web-api for the players with the given account ids
        /// </summary>
        /// <param name="accountId">Ids of the players to be searched for</param>
        /// <returns>Players that match the given account ids</returns>
        public List<Player> GetPlayersByAccountIds(IEnumerable<SelectorAccountId> accountIds)
        {
            IRestResponse response =  this.QueryPlayersEndpoint("filter[playerIds]", accountIds?.Select(_id => _id.Key));

            PlayerSearchResult result = JsonConvert.DeserializeObject<PlayerSearchResult>(response.Content);

            return result.Data;
        }

        /// <summary>
        /// Queries the PUBG-web-api for the players with the given ingame names
        /// </summary>
        /// <param name="accountId">Ingame names of the players to be searched for</param>
        /// <returns>Players that match the given ingame names</returns>
        public List<Player> GetPlayersByIngameNames(IEnumerable<string> ingameNames)
        {
            IRestResponse response = this.QueryPlayersEndpoint("filter[playerNames]", ingameNames);

            PlayerSearchResult result = JsonConvert.DeserializeObject<PlayerSearchResult>(response.Content);

            return result.Data;
        }

        /// <summary>
        /// Queries the PUBG-web-api for the match with the given id
        /// </summary>
        /// <param name="matchId">Id of the match to be searched for</param>
        /// <returns>Match with the given match id</returns>
        public Json<Match> GetMatch(SelectorMatchId matchId)
        {
            RestRequest request = this.CreateRequest($"matches/{matchId}", null);

            IRestResponse response = this.client.Execute(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestResponseException(response);
            }

            return new Json<Match>(response.Content);
        }


        /// <summary>
        /// Creates a new request to the PUBG-web-api for the specified resource with the specified query parameters
        /// </summary>
        protected virtual RestRequest CreateRequest(string resource, Dictionary<string, string> queryParameters)
        {
            RestRequest request = new RestRequest(resource, Method.GET, DataFormat.Json);
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddDecompressionMethod(DecompressionMethods.GZip);

            if(!(queryParameters is null) && queryParameters.Count > 0)
            {
                foreach(KeyValuePair<string, string> pair in queryParameters)
                {
                    request.AddQueryParameter(pair.Key, pair.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// Executes a query-request to the players-endpoint of the PUBG-web-api
        /// </summary>
        /// <param name="filter">Filter type "filter[playerIds]" or "filter[playerNames]"</param>
        /// <param name="filterValues">Identication values corresponding to the given 
        /// filter type (player id or ingame name)</param>
        /// <returns></returns>
        private IRestResponse QueryPlayersEndpoint(string filter, IEnumerable<string> filterValues)
        {
            if (filterValues?.Count() == 0)
            {
                throw new ArgumentException($"No values were specified for filter \"{filter}\" in the method parameter");
            }

            Dictionary<string, string> queryParameters = new Dictionary<string, string>() { { filter, String.Join(",", filterValues.Take(6)) } };
            RestRequest request = this.CreateRequest("players", queryParameters);

            IRestResponse response = this.rateLimiter.ExecuteRateLimitedRequest(this.client, request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new RestResponseException(response);
            }

            return response;
        }
    }
}