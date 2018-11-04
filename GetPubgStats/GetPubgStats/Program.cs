using System;
using System.Linq;


namespace GetPubgStats
{
    class Program
    {
        static void Main(string[] args)
        {
            Program.Test2();            
        }

        static void Test1()
        {
            Configuration.Data data = new Configuration.Data("config.json");

            PubgStatsController controller = new PubgStatsController( data.Get_DatabaseConnectionstring(), data.Get_PubgAPIKeys());
            controller.GetPlayerLastKills("gucki5",20);
            controller.GetPlayerLastKills("Hannes1909",20);
            controller.GetPlayerLastKills("ClawHunter",20);
            controller.UpdatePlayerstats();
        }

        static void Test2()
        {
            Configuration.Data data = new Configuration.Data("config.json");
            Database.DbLayer db = new Database.DbLayer(data.Get_DatabaseConnectionstring());

            Database.Models.Match match = db.GetMatchdata("0027a517-1b59-443b-8eed-40e86267c5ef");
            PubgAPI.Json<PubgAPI.Match> pubgmatch = new PubgAPI.Json<PubgAPI.Match>(match.Jsondata);
            var xxx = pubgmatch.AsObject();

        }

        static void Test3()
        {
            Configuration.Data data = new Configuration.Data("config.json");

            PubgAPI.PubgAPICalls pubgapi = new PubgAPI.PubgAPICalls();
            pubgapi.SetAPIKeys(data.Get_PubgAPIKeys());
            var player = pubgapi.GetPlayerData4Playername("gucki5");


        }

    }

}