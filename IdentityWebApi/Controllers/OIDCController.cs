using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace IdentityWebApi.Controllers
{

        [Authorize(AuthenticationSchemes = "Bearer")]
        [ApiController]
        [Route("api/app")]
        public class OIDCController : ControllerBase
        {

        [HttpGet]
            public IActionResult Get()
            {
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return Ok(claims);
            }
        }
}
