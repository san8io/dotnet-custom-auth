using System;
using Microsoft.Extensions.Options;

namespace custom_auth.Auth
{
    public class ApiKeyAuthenticationPostConfigureOptions : IPostConfigureOptions<ApiKeyAuthenticationOptions>
    {
        public void PostConfigure(string name, ApiKeyAuthenticationOptions options)
        {
            if (string.IsNullOrEmpty(options.Realm))
            {
                throw new InvalidOperationException("Realm must be provided in options");
            }
        }
    }
}
