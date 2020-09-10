using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace custom_auth.Auth
{
    public static class ApiKeyServiceExtensions
    {
        public static IServiceCollection AddApiKeyService(this IServiceCollection s)
        {
            s.AddSingleton(c =>
            {
                return new PasswordHasher<HashDummy>(null);
            });
            s.AddSingleton(c => {
                var config = c.GetRequiredService<IConfiguration>();
                return new ApiKeyRepository(config.GetConnectionString("Default"));
            });
            s.AddSingleton<IApikeyAuthenticationService, ApiKeyAuthenticationService>();
            return s;
        }

    }
}
