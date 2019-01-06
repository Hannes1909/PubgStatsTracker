using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PubgStatsWeb.Code.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task<AuthenticationScheme[]> GetExternalAuthenticationProviders(this HttpContext context)
        {
            if(context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IAuthenticationSchemeProvider provider = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();

            return (from scheme in await provider.GetAllSchemesAsync()
                    where !String.IsNullOrEmpty(scheme.DisplayName)
                    select scheme).ToArray();
        }

        public static async Task<bool> IsProviderSupportedAsync(this HttpContext context, string provider)
        {
            if(context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (from scheme in await context.GetExternalAuthenticationProviders()
                    where scheme.Name.Equals(provider, StringComparison.OrdinalIgnoreCase)
                    select scheme).Any();
        }
    }
}
