using System;
using System.Collections.Generic;
using System.Text;

namespace GetPubgStats
{
    class PubgPlayer
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<string> matches { get; set; }

    }

    class PubgMatch
    {
        public string id { get; set; }
        public string mapname { get; set; }
        public string gamemode { get; set; }
        public float duration { get; set; }
        public List<string> rosters { get; set; }

    }

    class PubgRoster
    {
        public string id { get; set; }
        public int rank { get; set; }
        public List<string> participantids { get; set; }
    }

    class PubgRosterParticipant
    {
        public string id { get; set; }
        public string playerid { get; set; }
        public string name { get; set; }
        public int DBNOs { get; set; }
        public int assists { get; set; }
        public int boosts { get; set; }
        public float damagedealt { get; set; }
        public string deathtype { get; set; }
        public int headshotkills { get; set; }
        public int heals { get; set; }
        public int kills { get; set; }
        public int killstreaks { get; set; }
        public float longestkill { get; set; }
        public int revives { get; set; }
        public float ridedistance { get; set; }
        public int roadkills { get; set; }
        public float swimdistance { get; set; }
        public int teamkills { get; set; }
        public float timesurvived { get; set; }
        public float walkdistance { get; set; }
        public int vehicledestroys { get; set; }
        public int weaponsacquired { get; set; }
    }

}
