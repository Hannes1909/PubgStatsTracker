using RestSharp.Authenticators;
using RestSharp;
using System;

namespace GetPubgStats.Rest.RateLimiting
{
    public class ApiAccessToken
    {
        private static readonly DateTime UtcStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private DateTime nextResetTime;

        public ApiAccessToken(string token, string tokenType)
        {
            this.AuthenticationHeader = new OAuth2AuthorizationRequestHeaderAuthenticator(token, tokenType);
            this.TokenValue = token;
            this.RateLimit = 10; //initial default rate limit will be updated after first request
        }

        public OAuth2AuthorizationRequestHeaderAuthenticator AuthenticationHeader { get; private set; }
        public string TokenValue { get; private set; }
        public int RemainingUses { get; private set; }
        public int RateLimit { get; private set; }

        public void Update(Parameter rateLimitHeader, Parameter rateLimitRemainingHeader, Parameter rateLimitResetTimerHeader)
        {
            Int64.TryParse(rateLimitResetTimerHeader.Value?.ToString() ?? "", out long resetTimerNanoseconds);
            Int32.TryParse(rateLimitRemainingHeader.Value?.ToString() ?? "", out int remainingUses);
            Int32.TryParse(rateLimitHeader.Value?.ToString() ?? "", out int rateLimit);

            this.nextResetTime = UtcStart.AddSeconds(resetTimerNanoseconds);
            this.RemainingUses = remainingUses;
            this.RateLimit = rateLimit;
        }

        public bool CanBeUsed
        {
            get
            {
                if(this.nextResetTime < DateTime.UtcNow)
                {
                    this.RemainingUses = this.RateLimit;
                }

                return this.RemainingUses > 0;
            }
        }
    }
}
