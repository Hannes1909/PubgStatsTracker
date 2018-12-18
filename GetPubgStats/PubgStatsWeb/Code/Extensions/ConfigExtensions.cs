using Microsoft.Extensions.Configuration;
using PubgStatsWeb.Code.Config;

namespace PubgStatsWeb.Code.Extensions
{
    public static class ConfigExtensions
    {
        public static TwitchOAuthConfig GetTwitchOAuthConfig(this IConfiguration config, string sectionName)
        {
            return config.GetSection(sectionName).Get<TwitchOAuthConfig>();
        }
    }
}
