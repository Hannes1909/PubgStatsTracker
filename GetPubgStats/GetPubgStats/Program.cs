using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;

namespace GetPubgStats
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Data data = new Configuration.Data("config.json");

            PubgAPI.PubgAPICalls pubgapi = new PubgAPI.PubgAPICalls();
            pubgapi.SetAPIKeys(data.Get_bubgAPIKeys());

            PubgAPI.Player player = pubgapi.GetPlayerData("Hannes1909");

            Console.WriteLine("Deserialized: " + player.id);
            Console.ReadLine();
        }

        

    }

}