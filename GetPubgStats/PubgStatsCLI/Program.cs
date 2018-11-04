using System;
using Controller;

namespace PubgStatsCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.Data data = new Configuration.Data("config.json");
            PubgStatsController controller = new PubgStatsController(data.Get_DatabaseConnectionstring(), data.Get_PubgAPIKeys());

            if (args.Length == 0)
            {
                do
                {
                    System.Console.WriteLine(@"
PubgStatsCLI Commands

01 = GetPlayerLastKills(%s)
11 = UpdatePlayerstats()
12 = UpdateActivePlayerstats()

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
            Configuration.Data data = new Configuration.Data("config.json");

            PubgStatsController controller = new PubgStatsController(data.Get_DatabaseConnectionstring(), data.Get_PubgAPIKeys());
            controller.GetPlayerLastKills("gucki5", 20);
            controller.GetPlayerLastKills("Hannes1909", 20);
            controller.GetPlayerLastKills("ClawHunter", 20);
            controller.UpdatePlayerstats();
        }

    }
}
