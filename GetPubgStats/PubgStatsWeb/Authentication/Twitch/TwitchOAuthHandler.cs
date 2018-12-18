using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net;
using RestSharp;
using System;

namespace PubgStatsWeb.Authentication.Twitch
{
    public class TwitchOAuthHandler : OAuthHandler<TwitchOAuthOptions>
    {
        public TwitchOAuthHandler(IOptionsMonitor<TwitchOAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        { }

        protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
        {
            string output = QueryHelpers.AddQueryString(this.Options.AuthorizationEndpoint, new Dictionary<string, string>()
            {
                { "force_verify", this.Options.AlwaysForceVerification.ToString().ToLower() },
                { "state", this.Options.StateDataFormat.Protect(properties) },
                { "scope", this.FormatScope(this.Options.Scope) },
                { "client_id", this.Options.ClientId },
                { "redirect_uri", redirectUri },
                { "response_type", "code" }
            });
            return output;
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            RestClient client = new RestClient(this.Options.UserInformationEndpoint);

            RestRequest request = new RestRequest("", Method.GET, DataFormat.Json);
            request.AddHeader("Authorization", $"Bearer {tokens.AccessToken}");

            IRestResponse response = client.Execute(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                this.Logger.LogError($"Error while retrieving users profile: {response.Content} Status: {response.StatusCode}, "
                                   + $"Headers: {String.Join(";", response.Headers.Select(_param => $"{_param.Name}, {_param.Value}"))}");

                throw new InvalidOperationException("Error while retrieving users profile");
            }

            JObject user = JObject.Parse(response.Content);

            OAuthCreatingTicketContext ticketContext = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, this.Context,
                                                            this.Scheme, this.Options, this.Backchannel, tokens, user);
            ticketContext.RunClaimActions(user);

            await Options.Events.CreatingTicket(ticketContext);

            return new AuthenticationTicket(ticketContext.Principal, ticketContext.Properties, this.Scheme.Name);
        }
    }
}
