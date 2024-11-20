using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIOIDCWindows.Controllers
{
    [Authorize(AuthenticationSchemes = "Negotiate")]
    [Route("api/[controller]")]
    [ApiController]
    public class WindowsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = $"{User.Identity?.Name}" });
        }
    }
}
