using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ThreatIntelligencePlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet("Hello")]
        public IActionResult Hello()
        {
            return Ok("Hello");
        }
    }
}
