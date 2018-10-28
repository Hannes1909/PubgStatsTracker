using GetPubgStats.Configuration;
using System.Reflection;
using System.IO;

namespace PubgStatsTests
{
    public class TestConfig
    {
        private static TestConfig instance;

        public static TestConfig Instance { get { return instance ?? (instance = new TestConfig()); } }

        private TestConfig()
        {
            string path = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "config.json");
            this.Config = new AppConfig(path);
        }

        public AppConfig Config { get; set; }
    }
}