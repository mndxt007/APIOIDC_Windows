using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.ComponentModel.DataAnnotations;

namespace APIOIDCWindows.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/user")]
    public class OIDCController : ControllerBase
    {
        

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(User.Identity?.Name);
        }
    }
}
