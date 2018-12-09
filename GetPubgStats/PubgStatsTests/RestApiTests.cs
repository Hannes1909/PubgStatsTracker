using PubgStatsController.Json.Models;
using PubgStatsController.Rest;
using System;
using Xunit;

namespace PubgStatsTests
{
    public class RestApiTests
    {
        private readonly PubgRestClient client;

        public RestApiTests()
        {
            this.client = new PubgRestClient(TestConfig.Instance.Config.Settings.PubgApiConfig.ApiBaseUrl, 
                                             TestConfig.Instance.Config.Settings.PubgApiConfig.ApiAccessTokens);
        }

        [Fact]
        public void ApiTests()
        {
            Assert.Equal(2, this.client.GetPlayersByIngameNames(new SelektorAccountid[] { "White-Mickey", "shroud" }).Count);

            Player playerQuery = client.GetPlayerByIngameName("Hannes1909");

            Assert.True(playerQuery.Relationships.Matches.Count > 0);

            Match matchQuery = this.client.GetMatch(playerQuery.Relationships.Matches[0].Id);

            Assert.True(matchQuery.Participants.Length > 0);

            Assert.NotNull(this.client.GetPlayerByPlayerId(new SelektorAccountid(playerQuery.AccountId)));

            bool runInException = false;

            try
            {
                for (int i = 0; i < 20; i++)
                {
                    this.client.GetPlayerByName("White-Mickey");
                    System.Threading.Tasks.Task.Delay(50).Wait();
                }
                runInException = false;
            }
            catch (RateLimitExceededException)
            {
                runInException = true;
            }

            Assert.True(runInException);
        }
    }
}
