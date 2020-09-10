using Microsoft.AspNetCore.Authentication;

namespace custom_auth.Auth
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Realm { get; set; }
    }
}
