using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SvR.AuthenticationDemo.Controllers
{
    /// <summary>
    /// This controller is used to demonstrate how to use the User property in your controllers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // If you only have one authentication scheme, the authentication handler will be called anyway. And thus fill the User property.
    // If you have more then one authentication scheme, you need to specify which scheme to use, or create a special default authorization policy.
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IConfiguration _configuration;
        
        /// <summary>
        /// UserController constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public UserController(IConfiguration configuration, ILogger<UserController> logger)
        {
            _configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// This endpoint will return the claims of the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("Details")]
        public Dictionary<string,string>? GetClaims()
        {
            return User.Claims?.ToDictionary(c => c.Type, c => c.Value);
        }

        /// <summary>
        /// Say hi to the current user.
        /// </summary>
        [HttpGet("hi")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public string Hi()
        {
            return $"Hi {User.Identity?.Name}\r\nRemote IP: {this.HttpContext.Connection.RemoteIpAddress}";
        }

        /// <summary>
        /// Secret endpoint, with details on how to get some free consulting.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Lets hope no own discoveres how to access this endpoint</remarks>
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
