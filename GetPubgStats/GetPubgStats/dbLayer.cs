using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using PubgAPI;

namespace Database
{
    public class DbLayer
    {
        Models.PubgDbContext dbc;
        public DbLayer( string Connectionstring)
        {
            this.dbc = new Models.PubgDbContext(Connectionstring);
            

            //this.dbc.Database.EnsureCreated();
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

        public IEnumerable<Models.Playerdetail> GetPlayerWithActiveRequests(TimeSpan RequestWithinTimeSpan)
        {
            return this.dbc.Players.AsNoTracking().Where( _rec => _rec.LastStatsRequest >= System.DateTime.Now.Add( -RequestWithinTimeSpan));
        } 


        public IEnumerable<Models.Playerdetail> GetPlayers()
        {
            return this.dbc.Players.AsNoTracking();
        }

        public void CreatePlayermatches(PubgAPI.SelektorAccountid Accountid, IEnumerable<PubgAPI.SelektorMatchid> Matchids)
        {
            foreach(PubgAPI.SelektorMatchid matchid in Matchids.AsStringArray().Except( this.dbc.Matches.Select(_rec => _rec.Matchid ) ) ) 
            {
                Models.Match match = new Models.Match();
                match.Matchid = matchid;
                this.dbc.Matches.Add( match );
            }
            Task<int> inserttaskmatches = this.dbc.SaveChangesAsync();

            foreach(PubgAPI.SelektorMatchid matchid in Matchids.Except( this.dbc.Playermatches.Where( _rec => _rec.Accountid == Accountid.Key ).Select( _rec => _rec.MatchidAsObject )))
            {
                Models.Playermatches playermatch = new Models.Playermatches();
                playermatch.Accountid = Accountid;
                playermatch.Matchid = matchid;

                this.dbc.Playermatches.Add( playermatch );
            }

            var _updates = inserttaskmatches.Result;
            this.dbc.SaveChanges();
        }

        public IEnumerable<PubgAPI.SelektorMatchid> GetMatchidsWithoutJson()
        {
            return this.dbc.Matches.Where( _rec => _rec.Jsondata == null).Select( _rec => new PubgAPI.SelektorMatchid( _rec.Matchid ) );
        }

        public void StoreMatchAndPlayersStats(IEnumerable<PubgAPI.Player> Players, Func<PubgAPI.SelektorMatchid, PubgAPI.Json<PubgAPI.Match>> FetchPubgMatch)
        {
            IEnumerable<PubgAPI.SelektorMatchid> matchids4AllPlayer = Players.SelectMany( _rec => _rec.relationships.matches.data.Select( _match => _match.id )).Distinct();

            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatasInDB = 
                            (from _rec in this.dbc.Matches
                             where matchids4AllPlayer.AsStringArray().Contains( _rec.Matchid)
                             select _rec
                            ).ToArray()
                            .Select(_rec => { return ( _rec.MatchidAsObject, new PubgAPI.Json<PubgAPI.Match>(_rec.Jsondata) ); } );

// (from _matchid in matchids4AllPlayer.Select(_a => _a.Value).Except(matchdatasInDB.Select(_rec => _rec.matchid.Value))
            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatasNotInDb =
                            (from _matchid in matchids4AllPlayer.Except(matchdatasInDB.Select(_rec => _rec.matchid))
                             select (_matchid, FetchPubgMatch(_matchid) )
                            ).Where( _a => _a.Item2 != null);

            IEnumerable<(PubgAPI.SelektorMatchid matchid, PubgAPI.Json<PubgAPI.Match> matchjsonobj)> matchdatas = matchdatasInDB.Concat( matchdatasNotInDb );

            foreach( var _pubgmatch in matchdatasNotInDb)
            {
                PubgAPI.Match.Matchdata.MatchAttributes matchattr = _pubgmatch.matchjsonobj.AsObject().data.attributes;

                Database.Models.Match match = new Models.Match()
                {
                    Matchid       = _pubgmatch.matchid,
                    CreatedAt     = matchattr.createdAt,
                    Duration      = matchattr.duration,
                    GameMode      = (Models.Match.MatchGameMode)matchattr.gameMode,
                    MapName       = (Models.Match.MatchMapName)matchattr.mapName,
                    IsCustomMatch = Convert.ToInt16(matchattr.isCustomMatch),
                    SeasonState   = (Models.Match.MatchSeasonState)matchattr.seasonState,
                    Jsondata      = _pubgmatch.matchjsonobj.Value
                };
                this.dbc.Matches.Add(match);
            }
            this.dbc.SaveChanges();

            IEnumerable<Database.Models.Playermatches> playermatches =
                        (from _rec in Players.SelectMany( _a => _a.relationships.matches.data.Select( _match => new { player = _a, matchid = _match.id } ))
                                    join _matchdata in matchdatas
                                      on _rec.matchid equals _matchdata.matchid
                                    let _playerstats = (PubgAPI.PlayerdataParticipant)_matchdata.matchjsonobj.AsObject()
                                            .included.FirstOrDefault(    _a => _a is PubgAPI.PlayerdataParticipant 
                                                                      && ((PubgAPI.PlayerdataParticipant)_a).attributes.stats.playerId == _rec.player.id) 
                                    select new Database.Models.Playermatches(){
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
                        );

            var _records2Add = playermatches.Where( _rec => this.dbc.Playermatches.FirstOrDefault( _recdb =>    _rec.Accountid == _recdb.Accountid 
                                                                                                                           && _rec.Matchid == _recdb.Matchid 
                                                                                                               ) == null );

            this.dbc.Playermatches.AddRange( _records2Add );
            this.dbc.SaveChanges();

            return;   
        }

        public Models.Match GetMatchdata( PubgAPI.SelektorMatchid Matchid )
        {
            return this.dbc.Matches.AsNoTracking().FirstOrDefault( _rec => _rec.Matchid == Matchid.Key );
        }

        public void StoreMatchdata( PubgAPI.SelektorMatchid Matchid, DateTime Matchdate, string Matchjsondata )
        {
            Models.Match match = this.dbc.Matches.First( _rec => _rec.Matchid == Matchid);
            if (match != null)
            {
                match.CreatedAt = Matchdate;
                match.Jsondata = Matchjsondata;
            } else {
                this.dbc.Matches.Add( new Models.Match() { Matchid = Matchid, Jsondata = Matchjsondata, CreatedAt = Matchdate });
            }
            this.dbc.SaveChanges();
        }

        public IEnumerable<(Database.Models.Playerdetail playerdetail, Database.Models.Playermatches)> GetLastXMatches(IEnumerable<string> Playernames)
        {
            return null;
        } 
    }

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
                optionsBuilder.UseMySQL(this.connectionstring);
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
                    entity.HasKey(e => new { e.Accountid, e.Matchid });
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


