using System;

namespace GetPubgStats.Rest.RateLimiting
{
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(RateLimiter source) 
            : base("You exceeded all uses of your access tokens before the reset timer elapsed")
        {
            this.RateLimiter = source;
        }

        public RateLimiter RateLimiter { get; set; }
    }
}
