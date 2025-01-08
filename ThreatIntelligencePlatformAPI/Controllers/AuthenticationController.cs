using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatIntelligencePlatform.Business.DTOs.Authentication;
using ThreatIntelligencePlatform.Business.Interfaces;

namespace ThreatIntelligencePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAppAuthenticationService _appAuthenticationService;
        
        public AuthenticationController(IAppAuthenticationService appAuthenticationService)
        {
            _appAuthenticationService = appAuthenticationService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Hello")]
        public IActionResult Hello()
        {
            return Ok("Hello");
        }
        
        [HttpPost("Signup")]
        public async Task<IActionResult> Signup(SignupDto dto)
        {
            var authResponse = await _appAuthenticationService.Signup(dto);
            return Ok(authResponse);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var authResponse = await _appAuthenticationService.Login(dto);
            return Ok(authResponse);
        }
    }
}
