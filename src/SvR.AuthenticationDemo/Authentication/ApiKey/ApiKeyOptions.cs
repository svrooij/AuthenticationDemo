using Microsoft.AspNetCore.Authentication;

namespace SvR.AuthenticationDemo.Authentication.ApiKey
{
    public class ApiKeyOptions : AuthenticationSchemeOptions
    {
        public string[] ApiKeys { get; set; }
        public string Scheme { get; set; } = ApiKeyDefaults.DefaultScheme;
    }
}
