using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using PubgStatsController.Rest.Models;
using Pomelo.EntityFrameworkCore.MySql;

namespace Database
{
    public class DbLayer
    {
        Models.PubgDbContext dbc;

        /// <summary>
        /// fetch Json-Object für a Matchid
        /// </summary>
        Func<SelectorMatchId, Json<Match>> func_fetchPubgMatch;

        public DbLayer( string Connectionstring, Func<SelectorMatchId, Json<Match>> FetchPubgMatch)
        {
            this.dbc = new Models.PubgDbContext(Connectionstring);
            this.func_fetchPubgMatch = FetchPubgMatch;
        }

        public Models.Playerdetail GetPlayer4Playername(string Name)
        {
            return this.dbc.Players.AsNoTracking().FirstOrDefault( _rec => _rec.Name == Name);
        }

        public void CreatePlayer(SelectorAccountId Accountid, string Name)
        {
            Models.Playerdetail player = this.dbc.Players.AsNoTracking().FirstOrDefault( _rec => _rec.Name == Name);
            if (player == null)
            {
                player = new Models.Playerdetail();
                player.Accountid = Accountid;
                player.Name = Name;

                this.dbc.Players.Add( player );
                this.dbc.SaveChanges();
            }

            return;
        }

        public void SetActiveRequest4Player(SelectorAccountId Accountid )
        {
            Models.Playerdetail player = this.dbc.Players.FirstOrDefault( _rec => _rec.Accountid == Accountid.Key );
            if (player != null)
            {
                player.LastStatsRequest = System.DateTime.Now;
                this.dbc.SaveChanges();
            }
        }

        public IQueryable<Models.Playerdetail> GetPlayerWithActiveRequests(TimeSpan RequestWithinTimeSpan)
        {
            DateTime whereDateTime = System.DateTime.Now.Add(-RequestWithinTimeSpan);
            return this.dbc.Players.AsNoTracking().Where( _rec => _rec.LastStatsRequest >= whereDateTime);
        } 


        public IQueryable<Models.Playerdetail> GetPlayers()
        {
            return this.dbc.Players.AsNoTracking();
        }

        public IQueryable<Models.Match> GetMatches()
        {
            return this.dbc.Matches.AsNoTracking();
        }

        public void SaveMatchdata2DB(IEnumerable<Json<Match>> matches)
        {
            foreach (var _pubgmatch in matches)
            {
                string matchid = _pubgmatch.AsObject().Data.MatchId;
                if (this.dbc.Matches.Where( _rec => _rec.Matchid == matchid).Count() == 0)
                {
                    Match.Matchdata.MatchAttributes matchattr = _pubgmatch.AsObject().Data.Attributes;

                    Database.Models.Match match = new Models.Match()
                    {
                        Matchid = matchid,
                        CreatedAt = matchattr.CreatedAt,
                        Duration = matchattr.Duration,
                        GameMode = (Models.Match.MatchGameMode)matchattr.GameMode,
                        MapName = (Models.Match.MatchMapName)matchattr.Map,
                        IsCustomMatch = Convert.ToInt16(matchattr.IsCustomMatch),
                        SeasonState = (Models.Match.MatchSeasonState?)matchattr.SeasonState,
                        Jsondata = _pubgmatch.Value
                    };
                    this.dbc.Matches.Add(match);
                }
            }
            this.dbc.SaveChanges();
        }

        /// <summary>
        /// retrieve matches and insert into database
        /// </summary>
        /// <param name="Macthids"></param>
        /// <returns>matchdata jsonobject</returns>
        public IEnumerable<(SelectorMatchId matchid, Json<Match> matchjsonobj)> FetchMatches(IEnumerable<SelectorMatchId> Macthids)
        {
            IEnumerable<(SelectorMatchId matchid, Json<Match> matchjsonobj)> matchdatasInDB =
                            (from _rec in this.dbc.Matches
                             where Macthids.AsStringArray().Contains(_rec.Matchid)
                             select new { matchid = new SelectorMatchId(_rec.Matchid), jsondata = _rec.Jsondata }
                            ).ToArray()
                            .Select(_rec => { return (_rec.matchid, new Json<Match>(_rec.jsondata)); });

            IEnumerable<(SelectorMatchId matchid, Json<Match> matchjsonobj)> matchdatasNotInDb =
                            (from _matchid in Macthids.Except(matchdatasInDB.Select(_rec => _rec.matchid))
                             select (_matchid, this.func_fetchPubgMatch(_matchid))
                            ).Where(_a => _a.Item2 != null);

            this.SaveMatchdata2DB(matchdatasNotInDb.Select(_a => _a.matchjsonobj));

            IEnumerable<(SelectorMatchId matchid, Json<Match> matchjsonobj)> matchdatas = matchdatasInDB.Concat(matchdatasNotInDb);

            return matchdatas;
        }

        /// <summary>
        /// refresh the last games for the players
        /// </summary>
        /// <param name="Players"></param>
        public void StoreMatchAndPlayersStats(IEnumerable<Player> Players)
        {
            IEnumerable<SelectorMatchId> matchids4AllPlayer = Players.SelectMany(_rec => _rec.Relationships.Matches.Data.Select(_match => _match.MatchId)).Distinct();

            IEnumerable<(SelectorMatchId matchid, Json<Match> matchjsonobj)> matchdatas = this.FetchMatches(matchids4AllPlayer);

            IEnumerable<Database.Models.Playermatches> playermatches =
                        (from _rec in Players.SelectMany( _a => _a.Relationships.Matches.Data.Select( _match => new { player = _a, matchid = _match.MatchId } ))
                                    join _matchdata in matchdatas
                                      on _rec.matchid equals _matchdata.matchid
                                    let _playerstats = (PlayerdataParticipant)_matchdata.matchjsonobj.AsObject()
                                            .Included.FirstOrDefault(    _a => _a is PlayerdataParticipant 
                                                                      && ((PlayerdataParticipant)_a).Attributes.Stats.PlayerId == _rec.player.AccountId) 
                                    select new Database.Models.Playermatches(){
                                        Participant       = _playerstats?.Id,
                                        Accountid         = _rec.player.AccountId,
                                        Matchid           = _rec.matchid,
                                        Assists           = _playerstats?.Attributes.Stats.Assists,
                                        Boosts            = _playerstats?.Attributes.Stats.Boosts,
                                        DBNOs             = _playerstats?.Attributes.Stats.DBNOs,
                                        DamageDealt       = _playerstats?.Attributes.Stats.DamageDealt,
                                        DeathType         = (Database.Models.Playermatches.PlayerDeathType)_playerstats?.Attributes.Stats.DeathType,
                                        HeadshotKills     = _playerstats?.Attributes.Stats.HeadshotKills,
                                        Heals             = _playerstats?.Attributes.Stats.Heals,
                                        KillPlace         = _playerstats?.Attributes.Stats.KillPlace,
                                        KillStreaks       = _playerstats?.Attributes.Stats.KillStreaks,
                                        Kills             = _playerstats?.Attributes.Stats.Kills,
                                        LastKillPoints    = _playerstats?.Attributes.Stats.LastKillPoints,
                                        LastWinPoints     = _playerstats?.Attributes.Stats.LastWinPoints,
                                        LongestKill       = _playerstats?.Attributes.Stats.LongestKill,
                                        MostDamage        = _playerstats?.Attributes.Stats.MostDamage,
                                        Revives           = _playerstats?.Attributes.Stats.Revives,
                                        RideDistance      = _playerstats?.Attributes.Stats.RideDistance,
                                        RoadKills         = _playerstats?.Attributes.Stats.RoadKills,
                                        SwimDistance      = _playerstats?.Attributes.Stats.SwimDistance,
                                        TeamKills         = _playerstats?.Attributes.Stats.TeamKills,
                                        TimeSurvived      = _playerstats?.Attributes.Stats.TimeSurvived,
                                        VehicleDestroys   = _playerstats?.Attributes.Stats.VehicleDestroys,
                                        WalkDistance      = _playerstats?.Attributes.Stats.WalkDistance,
                                        WeaponsAcquired   = _playerstats?.Attributes.Stats.PickedUpWeapons,
                                        WinPlace          = _playerstats?.Attributes.Stats.GameRank
                                    }
                        ).ToArray();

            var _participantList = playermatches.Select(_a => _a.Participant).ToArray();
            var _recordsinDB = (from _rec in this.dbc.Playermatches
                                where _participantList.Contains(_rec.Participant)
                                select _rec.Participant
                               ).ToArray();

            var _records2Add = playermatches.Where(_rec => !_recordsinDB.Contains(_rec.Participant)).ToArray();

            this.dbc.Playermatches.AddRange( _records2Add );
            this.dbc.SaveChanges();

            return;   
        }

        public IEnumerable<(SelectorAccountId player, IEnumerable<Database.Models.Playermatches>)> GetLastXMatches(IEnumerable<SelectorAccountId> Players)
        {
            var xxx = (from _playermatch in this.dbc.Playermatches
                       join _match in this.dbc.Matches
                         on _playermatch.Matchid equals _match.Matchid
                       where Players.Select(_a => _a.Key).Contains(_playermatch.Accountid)
                       select _playermatch
                      );
            return null;
        }

        public IEnumerable<PubgAPI.SelektorAccountid> GetTeamPlayersLastGame(PubgAPI.SelektorAccountid player)
        {
            var participant = (from _playermatch in this.dbc.Playermatches
                               join _match in this.dbc.Matches
                               on _playermatch.Matchid equals _match.Matchid
                               orderby _match.CreatedAt descending
                               where _playermatch.Accountid == player.Key
                               select _playermatch.Participant
                              ).FirstOrDefault();

            return (from _playermatches in this.dbc.Playermatches
                    where _playermatches.Participant == participant
                    select new PubgAPI.SelektorAccountid(_playermatches.Accountid)
                   );

        }

        //public IEnumerable<PubgAPI.SelektorMatchid> GetMatchidsWithoutJson()
        //{
        //    return this.dbc.Matches.Where( _rec => _rec.Jsondata == null).Select( _rec => new SelektorMatchid( _rec.Matchid ) );
        //}

        //public Models.Match GetMatchdata( SelektorMatchid Matchid )
        //{
        //    return this.dbc.Matches.AsNoTracking().FirstOrDefault( _rec => _rec.Matchid == Matchid.Key );
        //}

        //public void StoreMatchdata( SelektorMatchid Matchid, DateTime Matchdate, string Matchjsondata )
        //{
        //    Models.Match match = this.dbc.Matches.First( _rec => _rec.Matchid == Matchid);
        //    if (match != null)
        //    {
        //        match.CreatedAt = Matchdate;
        //        match.Jsondata = Matchjsondata;
        //    } else {
        //        this.dbc.Matches.Add( new Models.Match() { Matchid = Matchid, Jsondata = Matchjsondata, CreatedAt = Matchdate });
        //    }
        //    this.dbc.SaveChanges();
        //}


    }

    /// -----------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
    namespace Models
    {
        public class PubgDbContext : DbContext
        {
            string connectionstring;
            public PubgDbContext(string Connectionstring)
            {
                this.connectionstring = Connectionstring;
                
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseMySql(this.connectionstring);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Playerdetail>(entity =>
                {
                    entity.HasKey(e => e.Accountid);
                });

                modelBuilder.Entity<Match>(entity =>
                {
                    entity.HasKey(e => e.Matchid);
                });

                modelBuilder.Entity<Playermatches>(entity =>
                {
                    entity.HasKey(e => e.Participant);
                    entity.HasOne(e => e.Player);
                    entity.HasOne(e => e.Match);
                });
            }

            public DbSet<Match> Matches { get; set; }
            public DbSet<Playerdetail> Players { get; set; }
            public DbSet<Playermatches> Playermatches { get; set; }
        }

        [Table("Matches")]
        public class Match
        {
            [Column("Matchid", TypeName="varchar(40)")]
            public string Matchid { get; set; }
            public SelectorMatchId MatchidAsObject => new SelectorMatchId(this.Matchid);

            public DateTime? CreatedAt { get; set; } //Time this match object was stored in the API
            public int? Duration { get; set; } // Length of the match measured in seconds
            public MatchGameMode? GameMode { get; set; } // Game mode played Enum: [ duo, duo-fpp, solo, solo-fpp, squad, squad-fpp, normal-duo, normal-duo-fpp, normal-solo, normal-solo-fpp, normal-squad, normal-squad-fpp ]
            public enum MatchGameMode { duo, duo_fpp, solo, solo_fpp, squad, squad_fpp, normal_duo, normal_duo_fpp, normal_solo, normal_solo_fpp, normal_squad, normal_squad_fpp }
            public MatchMapName? MapName { get; set; } // Desert_Main, Erangel_Main, Savage_Main, Range_Main
            public enum MatchMapName { Desert_Main, Erangel_Main, Savage_Main, Range_Main }
            public int? IsCustomMatch { get; set; } // True if this match is a custom match
            public MatchSeasonState? SeasonState { get; set; } // The state of the season [ closed, prepare, progress ]
            public enum MatchSeasonState { closed, prepare, progress }
            [Column(TypeName="longtext")]
            public string Jsondata { get; set; }
        }
    

        [Table("Playerdetails")]
        public class Playerdetail
        {
            [Column("Accountid", TypeName = "varchar(40)")]
            public string Accountid { get; set; }
            public SelectorAccountId AccountidAsObject => new SelectorAccountId(this.Accountid);
            [Column("Name", TypeName="varchar(40)")]
            public string Name { get; set; }

            public DateTime? LastStatsRequest { get; set; }
        }    

        [Table("Playermatches")]
        public class Playermatches
        {
            [Column("Participant", TypeName = "varchar(40)")]
            public string Participant { get; set; }

            [Column("Accountid", TypeName = "varchar(40)")]
            public string Accountid { get; set; }
            public SelectorAccountId AccountidAsObject => new SelectorAccountId(this.Accountid);

            [Column("Matchid", TypeName = "varchar(40)")]
            public string Matchid { get; set; }
            public SelectorMatchId MatchidAsObject => new SelectorMatchId(this.Matchid);

            public int? DBNOs { get; set; } // Number of enemy players knocked
            public int? Assists { get; set; } // Number of enemy players this player damaged that were killed by teammates
            public int? Boosts { get; set; } // Number of boost items used
            public double? DamageDealt { get; set; } // Total damage dealt. Note: Self inflicted damage is subtracted
            public PlayerDeathType? DeathType { get; set; } // alive, byplayer, suicide, logout
            public enum PlayerDeathType {
                alive, byplayer, suicide, logout
            }
            public int? HeadshotKills { get; set; } // Number of enemy players killed with headshots
            public int? Heals { get; set; } // Number of healing items used
            public int? KillPlace { get; set; } // This player's rank in the match based on kills
            public int? KillStreaks { get; set; } // Total number of kill streaks
            public int? Kills { get; set; } // // Number of enemy players killed
            public int? LastKillPoints { get; set; } 
            public int? LastWinPoints { get; set; }
            public double? LongestKill { get; set; }
            public double? MostDamage  { get; set; } // Highest amount of damage dealt with a single attack
            public int? Revives { get; set; } // Number of times this player revived teammates
            public double? RideDistance { get; set; } // Total distance traveled in vehicles measured in meters
            public int? RoadKills { get; set; } // Number of kills while in a vehicle
            public double? SwimDistance { get; set; } // Total distance traveled while swimming measured in meters
            public int? TeamKills { get; set; } // Number of times this player killed a teammate
            public double? TimeSurvived { get; set; } // Amount of time survived measured in seconds
            public int? VehicleDestroys { get; set; } // Number of vehicles destroyed
            public double? WalkDistance { get; set; } // Total distance traveled on foot measured in meters
            public int? WeaponsAcquired { get; set; } // Number of weapons picked up
            public int? WinPlace { get; set; } // This player's placement in the match


            public virtual Playerdetail Player { get; set; }
            public virtual Match Match { get; set; }
        }    
    }
}


