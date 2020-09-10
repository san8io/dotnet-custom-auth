using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace custom_auth.Auth
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string API_KEY_HEADER_NAME = "X-API-Key";
        private readonly IApikeyAuthenticationService _authService;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApikeyAuthenticationService authService)
            : base(options, logger, encoder, clock)
        {
            _authService = authService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            string creds = Encoding.UTF8.GetString(Convert.FromBase64String(providedApiKey));
            ApiKey apiKey = JsonConvert.DeserializeObject<ApiKey>(creds);
            ApiKeyDB validApiKey = await _authService.IsValidApiKey(apiKey);
            if (validApiKey == null)
            {
                return AuthenticateResult.Fail("Invalid api key");
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim($"app.claims.{apiKey.ResourceId}", apiKey.Actions),
                new Claim(ClaimTypes.Name, validApiKey.Name),
                new Claim("Id", apiKey.Id.ToString())
            };
            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
           Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
           await base.HandleChallengeAsync(properties);
        }
    }
}
