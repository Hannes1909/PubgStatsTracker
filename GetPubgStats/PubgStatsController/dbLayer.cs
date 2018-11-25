using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using PubgAPI;
using Pomelo.EntityFrameworkCore.MySql;

namespace Database
{
    public class DbLayer
    {
        Models.PubgDbContext dbc;

        /// <summary>
        /// fetch Json-Object für a Matchid
        /// </summary>
        Func<PubgAPI.SelektorMatchid, PubgAPI.Json<PubgAPI.Match>> func_fetchPubgMatch;

        public DbLayer( string Connectionstring, Func<PubgAPI.SelektorMatchid, PubgAPI.Json<PubgAPI.Match>> FetchPubgMatch)
        {
            this.dbc = new Models.PubgDbContext(Connectionstring);
            this.func_fetchPubgMatch = FetchPubgMatch;
        }

        public Models.Playerdetail GetPlayer4Playername(string Name)
        {
            return this.dbc.Players.AsNoTracking().FirstOrDefault( _rec => _rec.Name == Name);
        }

        public void CreatePlayer(PubgAPI.SelektorAccountid Accountid, string Name)
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

        public void SetActiveRequest4Player(PubgAPI.SelektorAccountid Accountid )
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

        /// <summary>
        /// retrieve matches and insert into database
        /// </summary>
        /// <param name="Macthids"></param>
        /// <returns>matchdata jsonobject</returns>
        public IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> FetchMatches(IEnumerable<PubgAPI.SelektorMatchid> Macthids)
        {
            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatasInDB =
                            (from _rec in this.dbc.Matches
                             where Macthids.AsStringArray().Contains(_rec.Matchid)
                             select new { matchid = new PubgAPI.SelektorMatchid(_rec.Matchid), jsondata = _rec.Jsondata }
                            ).ToArray()
                            .Select(_rec => { return (_rec.matchid, new PubgAPI.Json<PubgAPI.Match>(_rec.jsondata)); });

            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatasNotInDb =
                            (from _matchid in Macthids.Except(matchdatasInDB.Select(_rec => _rec.matchid))
                             select (_matchid, this.func_fetchPubgMatch(_matchid))
                            ).Where(_a => _a.Item2 != null);


            foreach (var _pubgmatch in matchdatasNotInDb)
            {
                PubgAPI.Match.Matchdata.MatchAttributes matchattr = _pubgmatch.matchjsonobj.AsObject().data.attributes;

                Database.Models.Match match = new Models.Match()
                {
                    Matchid = _pubgmatch.matchid,
                    CreatedAt = matchattr.createdAt,
                    Duration = matchattr.duration,
                    GameMode = (Models.Match.MatchGameMode)matchattr.gameMode,
                    MapName = (Models.Match.MatchMapName)matchattr.mapName,
                    IsCustomMatch = Convert.ToInt16(matchattr.isCustomMatch),
                    SeasonState = (Models.Match.MatchSeasonState)matchattr.seasonState,
                    Jsondata = _pubgmatch.matchjsonobj.Value
                };
                this.dbc.Matches.Add(match);
            }
            this.dbc.SaveChanges();

            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatas = matchdatasInDB.Concat(matchdatasNotInDb);

            return matchdatas;
        }

        /// <summary>
        /// refresh the last games for the players
        /// </summary>
        /// <param name="Players"></param>
        public void StoreMatchAndPlayersStats(IEnumerable<PubgAPI.Player> Players)
        {
            IEnumerable<PubgAPI.SelektorMatchid> matchids4AllPlayer = Players.SelectMany(_rec => _rec.relationships.matches.data.Select(_match => _match.id)).Distinct();

            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatas = this.FetchMatches(matchids4AllPlayer);

            IEnumerable<Database.Models.Playermatches> playermatches =
                        (from _rec in Players.SelectMany( _a => _a.relationships.matches.data.Select( _match => new { player = _a, matchid = _match.id } ))
                                    join _matchdata in matchdatas
                                      on _rec.matchid equals _matchdata.matchid
                                    let _playerstats = (PubgAPI.PlayerdataParticipant)_matchdata.matchjsonobj.AsObject()
                                            .included.FirstOrDefault(    _a => _a is PubgAPI.PlayerdataParticipant 
                                                                      && ((PubgAPI.PlayerdataParticipant)_a).attributes.stats.playerId == _rec.player.id) 
                                    select new Database.Models.Playermatches(){
                                        Participant       = _playerstats?.id,
                                        Accountid         = _rec.player.id,
                                        Matchid           = _rec.matchid,
                                        Assists           = _playerstats?.attributes.stats.assists,
                                        Boosts            = _playerstats?.attributes.stats.boosts,
                                        DBNOs             = _playerstats?.attributes.stats.DBNOs,
                                        DamageDealt       = _playerstats?.attributes.stats.damageDealt,
                                        DeathType         = (Database.Models.Playermatches.PlayerDeathType)_playerstats?.attributes.stats.deathType,
                                        HeadshotKills     = _playerstats?.attributes.stats.headshotKills,
                                        Heals             = _playerstats?.attributes.stats.heals,
                                        KillPlace         = _playerstats?.attributes.stats.killPlace,
                                        KillStreaks       = _playerstats?.attributes.stats.killStreaks,
                                        Kills             = _playerstats?.attributes.stats.kills,
                                        LastKillPoints    = _playerstats?.attributes.stats.lastKillPoints,
                                        LastWinPoints     = _playerstats?.attributes.stats.lastWinPoints,
                                        LongestKill       = _playerstats?.attributes.stats.longestKill,
                                        MostDamage        = _playerstats?.attributes.stats.mostDamage,
                                        Revives           = _playerstats?.attributes.stats.revives,
                                        RideDistance      = _playerstats?.attributes.stats.rideDistance,
                                        RoadKills         = _playerstats?.attributes.stats.roadKills,
                                        SwimDistance      = _playerstats?.attributes.stats.swimDistance,
                                        TeamKills         = _playerstats?.attributes.stats.teamKills,
                                        TimeSurvived      = _playerstats?.attributes.stats.timeSurvived,
                                        VehicleDestroys   = _playerstats?.attributes.stats.vehicleDestroys,
                                        WalkDistance      = _playerstats?.attributes.stats.walkDistance,
                                        WeaponsAcquired   = _playerstats?.attributes.stats.weaponsAcquired,
                                        WinPlace          = _playerstats?.attributes.stats.winPlace
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

        public IEnumerable<(PubgAPI.SelektorAccountid player, IEnumerable<Database.Models.Playermatches>)> GetLastXMatches(IEnumerable<PubgAPI.SelektorAccountid> Players)
        {
            var xxx = (from _playermatch in this.dbc.Playermatches
                       join _match in this.dbc.Matches
                         on _playermatch.Matchid equals _match.Matchid
                       where Players.Select(_a => _a.Key).Contains(_playermatch.Accountid)
                       select _playermatch
                      );
            return null;
        }

        //public IEnumerable<PubgAPI.SelektorMatchid> GetMatchidsWithoutJson()
        //{
        //    return this.dbc.Matches.Where( _rec => _rec.Jsondata == null).Select( _rec => new PubgAPI.SelektorMatchid( _rec.Matchid ) );
        //}

        //public Models.Match GetMatchdata( PubgAPI.SelektorMatchid Matchid )
        //{
        //    return this.dbc.Matches.AsNoTracking().FirstOrDefault( _rec => _rec.Matchid == Matchid.Key );
        //}

        //public void StoreMatchdata( PubgAPI.SelektorMatchid Matchid, DateTime Matchdate, string Matchjsondata )
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
            public PubgAPI.SelektorMatchid MatchidAsObject => new PubgAPI.SelektorMatchid(this.Matchid);

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
            public PubgAPI.SelektorAccountid AccountidAsObject => new PubgAPI.SelektorAccountid(this.Accountid);
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
            public PubgAPI.SelektorAccountid AccountidAsObject => new PubgAPI.SelektorAccountid(this.Accountid);

            [Column("Matchid", TypeName = "varchar(40)")]
            public string Matchid { get; set; }
            public PubgAPI.SelektorMatchid MatchidAsObject => new PubgAPI.SelektorMatchid(this.Matchid);

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


