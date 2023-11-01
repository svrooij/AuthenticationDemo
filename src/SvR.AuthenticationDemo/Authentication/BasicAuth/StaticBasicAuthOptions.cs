using Microsoft.AspNetCore.Authentication;

namespace SvR.AuthenticationDemo.Authentication.BasicAuth
{
    public class StaticBasicAuthOptions : AuthenticationSchemeOptions
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string Scheme { get; set; } = BasicAuthDefaults.DefaultScheme;
    }
}
