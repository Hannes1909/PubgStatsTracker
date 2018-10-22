using System;
using System.Collections.Generic;
using System.Text;

namespace PubgAPI
{
    public class PlayerSearchResult
    {
        public List<Player> data { get; set; }


        public Links links { get; set; }
    }

    public class Player
    {
        public string type { get; set; }
        public string id { get; set; }
        public Attributes attributes { get; set; }
        public class Attributes
        {
            public string name { get; set; }
            public object stats { get; set; }
            public string titleId { get; set; }
            public string shardId { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public string patchVersion { get; set; }
        }


        public Relationships relationships { get; set; }
        public class Relationships
        {
            public Assets assets { get; set; }
            public class Assets
            {
                public List<object> data { get; set; }
            }

            public Matches matches { get; set; }
            public class Matches
            {
                public List<Match> data { get; set; }

                public class Match
                {
                    public string type { get; set; }
                    public string id { get; set; }
                }
            }
        }

        public Links links { get; set; }
        public Meta meta { get; set; }
        public class Meta
        {
        }
    }

    public class Links
    {
        public string self { get; set; }
        public string schema { get; set; }
    }

}
