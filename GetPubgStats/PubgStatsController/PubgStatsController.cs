using PubgStatsController.Rest.Models;
using PubgStatsController.Rest;
using System.Linq;
using Database;
using System;

namespace Controller
{
    public class PubgStatsController
    {
        private readonly PubgRestClient apiClient;
        private readonly DbLayer dbLayer;

        public PubgStatsController(string databaseConnectionString, string pubgApiBaseUrl, string[] pubgApiKeys)
        {
            this.dbLayer = new DbLayer(databaseConnectionString, this.FetchMatchdataFromPubgAPI);
            this.apiClient = new PubgRestClient(pubgApiBaseUrl, pubgApiKeys);
        }

        public int[] GetPlayerLastKills(string playerName, int count)
        {
            SelectorAccountId accountid;
            Database.Models.Playerdetail playeredetail = this.dbLayer.GetPlayer4Playername(playerName);
            if (playeredetail == null)
            {
                Player player = this.apiClient.GetPlayerByIngameName(playerName);
                this.dbLayer.CreatePlayer(player.AccountId, player.Attributes.Name);
                accountid = player.AccountId;
            }
            else
            {
                accountid = playeredetail.Accountid;
            }
            this.dbLayer.SetActiveRequest4Player(accountid);

            return null;
        }

        /// <summary>
        /// aktualisiert Playermatches, welche gerade angezeigt werden
        /// </summary>
        public void UpdateActivePlayerstats()
        {
            // last Request within 5min
            this.dbLayer.StoreMatchAndPlayersStats(this.apiClient.GetPlayersByAccountIds(this.dbLayer.GetPlayerWithActiveRequests(new TimeSpan(0, 5, 0)).Select(_rec => new SelectorAccountId(_rec.Accountid))));
        }

        /// <summary>
        /// aktualisiert Playermatches für alle Spieler, die in der Datenbank geführt werden
        /// </summary>
        public void UpdatePlayerstats()
        {
            this.dbLayer.StoreMatchAndPlayersStats(this.apiClient.GetPlayersByAccountIds(this.dbLayer.GetPlayers().Select(_rec => new SelectorAccountId(_rec.Accountid))));
        }

        public void FetchMatches(string matchIds)
        {
            this.dbLayer.FetchMatches(matchIds.Split(",").Select(_a => new SelectorMatchId(_a)));
        }


        public void ImportMatches(string filenames)
        {
            this.dbLayer.SaveMatchdata2DB((from _filename in System.IO.Directory.GetFiles(".", filenames)
                                      select new Json<Match>(System.IO.File.ReadAllText(_filename))
                                      )
                                    );
        }


        public void Test()
        {
            var _xxx = this.db.GetTeamPlayersLastGame(this.db.GetPlayer4Playername("gucki5").AccountidAsObject);
        }

        /// <summary>
        /// fetch matchdata from WebAPI
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        private Json<Match> FetchMatchdataFromPubgAPI(SelectorMatchId matchId)
        {
            try
            {
                return this.apiClient.GetMatch(matchId);
            }
            catch (Exception exp)
            {
                return null;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="Players"></param>
        //void update_players_Lastmatches(IEnumerable<Player> Players)
        //{
        //    IEnumerable<SelektorMatchid> matchids4AllPlayer = Players.SelectMany( _rec => _rec.relationships.matches.data.Select( _match => _match.id )).Distinct();
        //    //IEnumerable<Json<Match>> matchesJson = 
        //    //     this.db.GetMatchesAndStore(matchids, ((_matchid) => { return this.GetMatchData( _matchid ).Value; }));

        //    foreach( Player player in Players)
        //    {
        //        this.db.CreatePlayermatches(player.id, player.relationships.matches.data.Select( _rec => _rec.id ));
        //    }

        //    foreach( SelektorMatchid matchid in this.db.GetMatchidsWithoutJson())
        //    {
        //        Json<Match> jsonmatchdata = this.GetMatchData( matchid );
        //        Match match = jsonmatchdata.AsObject();

        //        this.db.StoreMatchdata( match.data.id, match.data.attributes.createdAt, jsonmatchdata.Value );
        //    }
        //}

    }

}