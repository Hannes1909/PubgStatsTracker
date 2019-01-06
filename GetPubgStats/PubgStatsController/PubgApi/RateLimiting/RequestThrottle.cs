using System.Linq;
using RestSharp;

namespace PubgStatsController.PubgApi.RateLimiting
{
    public class RateLimiter
    {
        private readonly ApiAccessToken[] accessTokens;

        public RateLimiter(string[] apiAccessTokens)
        {
            this.accessTokens = apiAccessTokens.Select(_token => new ApiAccessToken(_token, "Bearer")).ToArray();
        }

        public IRestResponse ExecuteRateLimitedRequest(RestClient client, IRestRequest request)
        {
            ApiAccessToken usedToken = this.GetUsableAccessToken();

            client.Authenticator = usedToken.AuthenticationHeader;

            IRestResponse response = client.Execute(request);

            if (response.Headers.Count(_header => _header.Name.StartsWith("X-Ratelimit")) == 3)
            {
                usedToken.Update(response.Headers.Single(_header => _header.Name.Equals("X-Ratelimit-Limit")),
                                 response.Headers.Single(_header => _header.Name.Equals("X-Ratelimit-Remaining")),
                                 response.Headers.Single(_header => _header.Name.Equals("X-Ratelimit-Reset"))
                                );
            }

            return response;
        }

        private ApiAccessToken GetUsableAccessToken()
        {
            int maxUses = 0;

            if (this.accessTokens.Any(_token => _token.CanBeUsed))
            {
                maxUses = accessTokens.Where(_token => _token.CanBeUsed).Max(_token => _token.RemainingUses);
            }

            if (maxUses == 0)
            {
                throw new RateLimitExceededException(this);
            }

            return this.accessTokens.FirstOrDefault(_token => _token.RemainingUses == maxUses);
        }
    }
}
