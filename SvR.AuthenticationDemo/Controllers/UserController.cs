using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SvR.AuthenticationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // If you only have one authentication scheme, the authentication handlers will be called anyway. And thus fill the User property.
    // If you have more then one authentication scheme, you need to specify which scheme to use.
    [Authorize]
    public class UserController : ControllerBase
    {
        [HttpGet("Details")]
        public Dictionary<string,string>? GetClaims()
        {
            return User.Claims?.ToDictionary(c => c.Type, c => c.Value);
        }
    }
}
