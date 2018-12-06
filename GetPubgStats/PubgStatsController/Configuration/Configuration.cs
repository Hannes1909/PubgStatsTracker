namespace PubgStatsController.Configuration
{
    public class Configuration
    {
        public PubgApiConfig PubgApi { get; set; }
        public DbLayerConfig DbLayer { get; set; }
    }

    public class PubgApiConfig
    {
        public string ApiBaseUrl { get; set; }
        public string[] ApiKeys { get; set; }
    }

    public class DbLayerConfig
    {
        public string ConnectionString { get; set; }
    }
}
