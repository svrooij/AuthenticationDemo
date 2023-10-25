using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SvR.AuthenticationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Details")]
        public Dictionary<string,string>? GetClaims()
        {
            return User.Claims?.ToDictionary(c => c.Type, c => c.Value);
        }
    }
}
