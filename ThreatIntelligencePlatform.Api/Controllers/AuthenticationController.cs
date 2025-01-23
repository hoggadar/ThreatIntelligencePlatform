using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatIntelligencePlatform.Business.DTOs.Authentication;
using ThreatIntelligencePlatform.Business.Interfaces;

namespace ThreatIntelligencePlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAppAuthenticationService _appAuthenticationService;
        private readonly ILogger<AuthenticationController> _logger;
        
        public AuthenticationController(IAppAuthenticationService appAuthenticationService, ILogger<AuthenticationController> logger)
        {
            _appAuthenticationService = appAuthenticationService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Hello")]
        public IActionResult Hello()
        {
            _logger.LogInformation("Hello");
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
