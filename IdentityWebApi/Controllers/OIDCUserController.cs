using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace IdentityWebApi.Controllers
{
    [Authorize(AuthenticationSchemes ="Bearer")]
    [ApiController]
    [Route("api/user")]
    public class OIDCUserController : Controller
    {
        const string scopeRequiredByApi = "Api";
        [RequiredScope(scopeRequiredByApi)]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(User.Identity?.Name);
        }
    }
}
