using System;
using System.Linq;


namespace GetPubgStats
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Data data = new Configuration.Data("config.json");

            PubgAPI.PubgAPICalls pubgapi = new PubgAPI.PubgAPICalls();
            pubgapi.SetAPIKeys(data.Get_PubgAPIKeys());

            PubgAPI.Player player = pubgapi.GetPlayerData("Hannes1909");
            Console.WriteLine($"accountid for Hannes1909: {player?.id}" );

            (string matchjson, PubgAPI.Match match) = pubgapi.GetMatchData("ce0fabe5-0b03-4c8d-b706-101507a3d19b");
            int? _place = (from _participant in match.included.OfType<PubgAPI.PlayerdataParticipant>()
                          where _participant.attributes.stats.playerId == player.id
                          select _participant.attributes.stats.winPlace
                         ).FirstOrDefault();
            Console.WriteLine($"{_place}. Place for Hannes1909 in match 'ce0fabe5-0b03-4c8d-b706-101507a3d19b'");


            Console.ReadLine();
        }

        

    }

}