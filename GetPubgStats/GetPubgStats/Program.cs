using GetPubgStats.Configuration;
using GetPubgStats.Rest.Models;
using GetPubgStats.Rest;
using System.Reflection;
using System.Linq;
using System.IO;
using System;

namespace GetPubgStats
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfig config = new AppConfig(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "config.json"));
            PubgRestClient restClient = new PubgRestClient(config.Settings.ApiBaseUrl, config.Settings.ApiAccessTokens);

            PlayersQueryResult playerQuery = restClient.GetPlayerByName("Hannes1909");
            Player player = playerQuery.Players[0];

            Console.WriteLine($"Id of {player.Attributes.Username} is {player.AccountId}");
            Console.WriteLine($"{player.Attributes.Username} participated in {player.Relationships.Matches.Length} matches in the past 14 days");

            if (player.Relationships.Matches.Length > 0)
            {
                MatchQueryResult match = restClient.GetMatch(player.Relationships.Matches[0].Id);
                Participant participant = match.Participants.SingleOrDefault(_part => _part.Attributes.Stats?.PlayerId?.Equals(player.AccountId) ?? false);

                if (participant != null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{player.Attributes.Username}'s last match was at {match.Data.Attributes.CreatedAt}");
                    Console.WriteLine($"{player.Attributes.Username} killed {participant.Attributes.Stats.KillCount} players");
                    Console.WriteLine($"{player.Attributes.Username} died of cause {participant.Attributes.Stats.DeathType}");
                    Console.WriteLine($"{player.Attributes.Username}'s match rank was {participant.Attributes.Stats.MatchRank}\n");
                }
            }

            Console.WriteLine("Execution finished, press any key to continue...");
            Console.ReadLine();
        }
    }
}