using GetPubgStats.Rest.RateLimiting;
using GetPubgStats.Rest.Models;
using GetPubgStats.Rest;
using PubgStatsTests;
using Xunit;

namespace PubgStatsTest
{
    public class RestApiTests
    {
        private readonly PubgRestClient client;

        public RestApiTests()
        {
            this.client = new PubgRestClient(TestConfig.Instance.Config.Settings.ApiBaseUrl, TestConfig.Instance.Config.Settings.ApiAccessTokens);
        }

        [Fact]
        public void ApiTests()
        {
            PlayersQueryResult playerQuery = client.GetPlayerByName("Hannes1909");
            Assert.Equal(2, this.client.GetPlayersByName(new string[] { "White-Mickey", "shroud" }).Players.Length);

            Assert.True(playerQuery.Players[0].Relationships.Matches.Length > 0);

            MatchQueryResult matchQuery = this.client.GetMatch(playerQuery.Players[0].Relationships.Matches[0].Id);

            Assert.True(matchQuery.Participants.Length > 0);

            Assert.NotNull(this.client.GetPlayerById(playerQuery.Players[0].AccountId));

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
