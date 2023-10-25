using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace SvR.AuthenticationDemo.Authentication.ApiKey
{
    public class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
    {
        private readonly IOptionsMonitor<ApiKeyOptions> _optionsMonitor;
        private readonly ILogger<ApiKeyHandler> _logger;

        public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _optionsMonitor = options;
            _logger = logger.CreateLogger<ApiKeyHandler>();
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var options = _optionsMonitor.CurrentValue;
            var apiKeyHeader = Request.Headers["X-API-Key"].ToString();
            if (!string.IsNullOrEmpty(apiKeyHeader))
            {
                // This is off course the point where you would validate the API key in the database or somewhere else.
                // For demo purposes we just check if the API key is in the list of valid API keys.
                if (options.ApiKeys.Contains(apiKeyHeader))
                {
                    // Do not do this in production, protect your secrets!
                    _logger.LogInformation("API key {ApiKey} is valid", apiKeyHeader);
                    // If you check the api key from a database, you can add details about the application to the claims.
                    var claims = new List<Claim>
                    {
                        new Claim("api", apiKeyHeader),
                    };
                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(claims, options.Scheme)), options.Scheme)));

                }

                _logger.LogInformation("API key {ApiKey} is invalid", apiKeyHeader);
                return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
            }

            // To support more then 1 authentication scheme, you need to return AuthenticateResult.NoResult() if you can't authenticate the user.
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}
