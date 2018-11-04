using System;
using System.Collections.Generic;
using System.Linq;


namespace Controller
{
    public class PubgStatsController
    {
        Database.DbLayer db;
        PubgAPI.PubgAPICalls pubgapi;

        public PubgStatsController(string DbConnectionstring, string[] PubgAPIKeys)
        {
            this.db = new Database.DbLayer(DbConnectionstring);

            this.pubgapi = new PubgAPI.PubgAPICalls();
            this.pubgapi.SetAPIKeys(PubgAPIKeys);
        }

        public int[] GetPlayerLastKills(string Playername, int Count)
        {
            PubgAPI.SelektorAccountid accountid;
            Database.Models.Playerdetail playeredetail = this.db.GetPlayer4Playername( Playername );
            if (playeredetail == null)
            {
                PubgAPI.Player player = pubgapi.GetPlayerData4Playername(Playername);
                this.db.CreatePlayer( player.id, player.attributes.name );
                accountid = player.id;
            } else {
                accountid = playeredetail.Accountid;
            }
            this.db.SetActiveRequest4Player( accountid );

            return null;
        }

        public void UpdateActivePlayerstats()
        {
            // last Request within 5min
            this.update_players_Lastmatches(this.pubgapi.GetPlayerData( this.db.GetPlayerWithActiveRequests( new TimeSpan(0,5,0)).Select( _rec => _rec.AccountidAsObject )));
        }

        public void UpdatePlayerstats()
        {
            Func<PubgAPI.SelektorMatchid, PubgAPI.Json<PubgAPI.Match>> _funcFetchMatchdata = (_matchid => {
                try
                {
                    return this.pubgapi.GetMatchData( _matchid );
                }
                catch
                {
                    return null;
                }
            });


            this.db.StoreMatchAndPlayersStats( this.pubgapi.GetPlayerData(this.db.GetPlayers().Select( _rec => _rec.AccountidAsObject )), _funcFetchMatchdata );
        }

        void update_players_Lastmatches(IEnumerable<PubgAPI.Player> Players)
        {
            IEnumerable<PubgAPI.SelektorMatchid> matchids4AllPlayer = Players.SelectMany( _rec => _rec.relationships.matches.data.Select( _match => _match.id )).Distinct();
            //IEnumerable<PubgAPI.Json<PubgAPI.Match>> matchesJson = 
            //     this.db.GetMatchesAndStore(matchids, ((_matchid) => { return this.pubgapi.GetMatchData( _matchid ).Value; }));

            foreach( PubgAPI.Player player in Players)
            {
                this.db.CreatePlayermatches(player.id, player.relationships.matches.data.Select( _rec => _rec.id ));
            }

            foreach( PubgAPI.SelektorMatchid matchid in this.db.GetMatchidsWithoutJson())
            {
                PubgAPI.Json<PubgAPI.Match> jsonmatchdata = this.pubgapi.GetMatchData( matchid );
                PubgAPI.Match match = jsonmatchdata.AsObject();

                this.db.StoreMatchdata( match.data.id, match.data.attributes.createdAt, jsonmatchdata.Value );
            }
        }

    }

}