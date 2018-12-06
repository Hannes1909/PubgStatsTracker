using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace PubgStatsController.Configuration
{
    public class AppConfig
    {
        private readonly string filePath;
        private Configuration config;

        public AppConfig(string filePath)
        {
            this.filePath = filePath;

            if (!File.Exists(filePath))
            {
                this.RestoreDefaults();
            }

            ConfigurationBuilder configuration = new ConfigurationBuilder();
            configuration.SetBasePath(Directory.GetCurrentDirectory());
            configuration.AddJsonFile(filePath);
            this.config = configuration.Build().Get<Configuration>();
        }

        /// <summary>
        /// Saves all changes to the loaded configuration
        /// </summary>
        public void Save()
        {
            lock (this.config)
            {
                File.WriteAllText(this.filePath, JsonConvert.SerializeObject(this.config, Formatting.Indented));
            }
        }

        /// <summary>
        /// Restores the default config values and saves them to the config file
        /// </summary>
        public void RestoreDefaults()
        {
            this.config = new Configuration()
            {
                PubgApi = new PubgApiConfig()
                {
                    ApiBaseUrl = "https://api.pubg.com/shards/pc-eu",
                    ApiKeys = new string[] { "key1", "key2" }
                },
                DbLayer = new DbLayerConfig()
                {
                    ConnectionString = "SERVER=127.0.0.1; DATABASE = xxxx; UID = xxxx; PASSWORD = xxxxxxxx;"
                }
            };

            this.Save();
        }

        public string PubgApiBaseUrl { get { return this.config?.PubgApi.ApiBaseUrl; } }
        public string[] PubgApiKeys { get { return this.config?.PubgApi.ApiKeys; } }

        public string DbLayerConnectionString { get { return this.config?.DbLayer.ConnectionString; } }
    }
}
