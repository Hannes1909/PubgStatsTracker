using PubgStatsController.Configuration;
using PubgStatsController;
using Controller;
using System;

namespace PubgStatsCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfig config = new AppConfig("config.json");
            Controller.PubgStatsController controller = new Controller.PubgStatsController(config.DbLayerConnectionString, config.PubgApiBaseUrl, config.PubgApiKeys);

            if (args.Length == 0)
            {
                do
                {
                    System.Console.WriteLine(@"
PubgStatsCLI Commands

01 = GetPlayerLastKills(%s)
11 = UpdatePlayerstats()
12 = UpdateActivePlayerstats()
21 = FetchMatchdata(%s)
29 = ImportMatchdata(%s)

                ");

                    System.Console.Write("Command: ");
                    string cmd = System.Console.ReadLine();
                    string playername;

                    switch (Convert.ToInt16(cmd))
                    {
                        case 1:
                            System.Console.Write("Player: ");
                            playername = System.Console.ReadLine();
                            controller.GetPlayerLastKills(playername, 1);

                            break;

                        case 11:
                            controller.UpdatePlayerstats();
                            break;

                        case 12:
                            controller.UpdateActivePlayerstats();
                            break;

                        case 21:
                            System.Console.Write("Matchid: ");
                            string matches = System.Console.ReadLine();
                            controller.FetchMatches(matches);
                            break;

                        case 29:
                            System.Console.Write("Filename (*.match)");
                            string matchfilemask = System.Console.ReadLine();
                            controller.ImportMatches(matchfilemask);

                            break;

                        case 99:
                            controller.Test();

                            break;

                        default:
                            return;
                    }

                } while (true);
            }
            else
            {
                switch (args.Length)
                {
                    case 1:
                        switch (args[0])
                        {
                            case "UpdatePlayerstats":
                                controller.UpdatePlayerstats();
                                break;

                            case "UpdateActivePlayerstats":
                                controller.UpdateActivePlayerstats();
                                break;

                            default:
                                break;
                        }
                        break;
                }
            }
        }

        static void Test1()
        {
            AppConfig config = new AppConfig("config.json");

            Controller.PubgStatsController controller = new Controller.PubgStatsController(config.DbLayerConnectionString, config.PubgApiBaseUrl, config.PubgApiKeys);
            controller.GetPlayerLastKills("gucki5", 20);
            controller.GetPlayerLastKills("Hannes1909", 20);
            controller.GetPlayerLastKills("ClawHunter", 20);
            controller.UpdatePlayerstats();
        }

    }
}
