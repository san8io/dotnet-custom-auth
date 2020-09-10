using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace custom_auth.Auth
{
    public static class ApiKeyAuthenticationExtensions
    {
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
            => builder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme);
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme)
            => builder.AddApiKey(authenticationScheme, configureOptions: null);
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> configureOptions)
            => builder.AddApiKey(ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);
        public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyAuthenticationOptions> configureOptions)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Services.AddSingleton<IPostConfigureOptions<ApiKeyAuthenticationOptions>, ApiKeyAuthenticationPostConfigureOptions>();
            builder.Services.AddApiKeyService();

            return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                authenticationScheme, configureOptions);
        }
    }
}
