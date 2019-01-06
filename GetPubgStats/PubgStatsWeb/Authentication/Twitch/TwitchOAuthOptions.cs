using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;

namespace PubgStatsWeb.Authentication.Twitch
{
    public class TwitchOAuthOptions : OAuthOptions
    {
        public TwitchOAuthOptions()
        {
            this.AuthorizationEndpoint = "https://id.twitch.tv/oauth2/authorize";
            this.UserInformationEndpoint = "https://api.twitch.tv/helix/users";
            this.TokenEndpoint = "https://id.twitch.tv/oauth2/token";
            this.ClaimsIssuer = "Twitch";

            this.ClaimActions.MapCustomJson(ClaimTypes.NameIdentifier, _user =>
            {
                return this.GetUserProperty(_user, "id");
            });

            this.ClaimActions.MapCustomJson(ClaimTypes.Name, _user =>
            {
                return this.GetUserProperty(_user, "login");
            });

            this.ClaimActions.MapCustomJson(ClaimTypes.Email, _user =>
            {
                return this.GetUserProperty(_user, "email");
            });


            this.ClaimActions.MapCustomJson(TwitchClaims.DisplayName, _user =>
            {
                return this.GetUserProperty(_user, "display_name");
            });

            this.ClaimActions.MapCustomJson(TwitchClaims.Type, _user =>
            {
                return this.GetUserProperty(_user, "type");
            });

            this.ClaimActions.MapCustomJson(TwitchClaims.BroadcasterType, _user =>
            {
                return this.GetUserProperty(_user, "broadcaster_type");
            });

            this.ClaimActions.MapCustomJson(TwitchClaims.Description, _user =>
            {
                return this.GetUserProperty(_user, "description");
            });

            this.ClaimActions.MapCustomJson(TwitchClaims.OfflineImageUrl, _user =>
            {
                return this.GetUserProperty(_user, "offline_image_url");
            });

            this.ClaimActions.MapCustomJson(TwitchClaims.ProfileImageUrl, _user =>
            {
                return this.GetUserProperty(_user, "profile_image_url");
            });
        }

        private string GetUserProperty(JObject user, string property)
        {
            JToken inner = user["data"]?.FirstOrDefault();

            if(!(inner is null))
            {
                return inner.Value<string>(property);
            }
            return String.Empty;
        }

        public bool AlwaysForceVerification { get; set; }
    }

    public static class TwitchClaims
    {
        public const string BroadcasterType = "urn:twitch:broadcastertype";
        public const string OfflineImageUrl = "urn:twitch:offlineimageurl";
        public const string ProfileImageUrl = "urn:twitch:profileimageurl";
        public const string Description = "urn:twitch:description";
        public const string DisplayName = "urn:twitch:displayname";
        public const string Type = "urn:twitch:type";
    }
}
