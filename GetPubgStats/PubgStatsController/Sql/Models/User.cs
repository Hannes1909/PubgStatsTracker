namespace PubgStatsController.Sql.Models
{
    public class User
    {
        public long Id { get; set; }
        public string TwitchUserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
