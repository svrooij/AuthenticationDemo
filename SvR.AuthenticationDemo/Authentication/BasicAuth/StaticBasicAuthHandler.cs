using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace SvR.AuthenticationDemo.Authentication.BasicAuth
{
    /// <summary>
    /// Static Basic Auth Handler, DO NOT USE IN PRODUCTION!!!
    /// </summary>
    /// <remarks>
    /// This handler is just for demo purposes, it is not secure and should not be used in production.
    /// </remarks>
    public class StaticBasicAuthHandler : AuthenticationHandler<StaticBasicAuthOptions>
    {
        private readonly IOptionsMonitor<StaticBasicAuthOptions> _optionsMonitor;
        private readonly ILogger<StaticBasicAuthHandler> _logger;

        public StaticBasicAuthHandler(IOptionsMonitor<StaticBasicAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _optionsMonitor = options;
            _logger = logger.CreateLogger<StaticBasicAuthHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var options = _optionsMonitor.CurrentValue;
            var authorizationHeaderValue = Request.Headers.Authorization.ToString();
            if (!string.IsNullOrEmpty(authorizationHeaderValue) && AuthenticationHeaderValue.TryParse(authorizationHeaderValue, out var parsedHeader) && parsedHeader.Scheme.Equals(options.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                var creds = Encoding.UTF8.GetString(Convert.FromBase64String(parsedHeader.Parameter!)).Split(':', 2);
                // You now have the username and password in the creds array
                // Validate the username and password, with your own method, and return a ClaimsPrincipal if it's valid
                if(creds.Length == 2 && creds[0].Equals(options.Username, StringComparison.OrdinalIgnoreCase) && creds[1] == options.Password)
                {
                    // Create a ClaimsPrincipal with the claims you want to use for authorization
                    var claims = new List<Claim>
                    {
                        new Claim("sub", creds[0])
                    };
                    // Return the correct AuthenticationResult
                    return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, options.Scheme)), options.Scheme));
                }

                return AuthenticateResult.Fail("Invalid username or password");
            }

            return AuthenticateResult.NoResult();
            
        }
    }
}
