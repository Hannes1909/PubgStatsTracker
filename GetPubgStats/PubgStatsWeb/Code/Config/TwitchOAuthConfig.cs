namespace PubgStatsWeb.Code.Config
{
    public class TwitchOAuthConfig
    {
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public bool ForceVerify { get; set; }
        public string ClientId { get; set; }
        public string[] Scopes { get; set; }
    }
}
