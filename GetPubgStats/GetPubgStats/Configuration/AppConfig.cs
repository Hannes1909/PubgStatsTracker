using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

namespace GetPubgStats.Configuration
{
    public class AppConfig
    {
        private readonly string filePath;

        public AppConfig(string filePath)
        {
            this.filePath = filePath;

            if (File.Exists(this.filePath))
            {
                this.Reload();
            }
            else
            {
                this.RestoreDefaultConfig();
            }
        }

        public Settings Settings { get; private set; }

        /// <summary>
        /// Reloads the configuration from the config-file
        /// </summary>
        public void Reload()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.SetBasePath(new FileInfo(this.filePath).Directory.FullName);
            this.Settings = builder.AddJsonFile(this.filePath).Build().Get<Settings>();
        }

        /// <summary>
        /// Saves the configuration to the config-file
        /// </summary>
        public void Save()
        {
            File.WriteAllText(this.filePath, JsonConvert.SerializeObject(this.Settings, Formatting.Indented));
        }

        /// <summary>
        /// Saves the default configuration to the config-file
        /// </summary>
        public void RestoreDefaultConfig()
        {
            this.Settings = new Settings()
            {
                ApiBaseUrl = "https://api.pubg.com/shards/pc-eu",
                ApiAccessTokens = new string[] { "key1", "key2" }
            };

            this.Save();
        }
    }

    public class Settings
    {
        public string[] ApiAccessTokens { get; set; }

        public string ApiBaseUrl { get; set; }
    }
}
