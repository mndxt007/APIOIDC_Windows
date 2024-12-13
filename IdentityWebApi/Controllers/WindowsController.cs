using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebApi.Controllers
{
    [Authorize(AuthenticationSchemes = "Negotiate")]
    [Route("api/[controller]")]
    [ApiController]
    public class WindowsController : ControllerBase
    {
        //test using Windows Authentication - curl https://localhost:7036/api/windows --negotiate
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { Message = $"{User.Identity?.Name}" });
        }
    }
}
