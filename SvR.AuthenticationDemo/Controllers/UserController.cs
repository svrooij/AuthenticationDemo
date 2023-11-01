using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SvR.AuthenticationDemo.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    // If you only have one authentication scheme, the authentication handler will be called anyway. And thus fill the User property.
    // If you have more then one authentication scheme, you need to specify which scheme to use, or create a special default authorization policy.
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _configuration = configuration;
            this.logger = logger;
        }

        [HttpGet("Details")]
        public Dictionary<string,string>? GetClaims()
        {
            return User.Claims?.ToDictionary(c => c.Type, c => c.Value);
        }

        [HttpGet("Owner")]
        [Authorize("SecureApp")]
        public string GetOwner()
        {
            // This claim will have the value of `1` if the application uses a client secret for authentication
            // This claim will have the value of `2` if the application uses certificates (or managed identity) for authentication
            // I would normally do this in the policy, but that would just show a 403 forbidden error, and not a nice hint on how to get access.
            var clientSecurity = User.FindFirstValue("azpacr");

            if (clientSecurity != "2")
            {
                return "Nice try, but this endpoints requires you to use the really secure authentication method (certificates or managed identity)!";
            }

            var tenantId = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");
            logger.LogWarning("Someone accessed the owner endpoint from tenant {TenantId}", tenantId);
            var message = "The first one who will send me an email at: "
                + _configuration["OwnerEmail"]
                +" with a picture of their conference badge from an email at tenant: "
                + tenantId
                + " will get 2 hours of free consulting on this topic!";
            return message;
        }
    }
}
