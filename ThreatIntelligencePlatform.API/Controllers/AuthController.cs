using Microsoft.AspNetCore.Mvc;
using ThreatIntelligencePlatform.Business.DTOs.Auth;
using ThreatIntelligencePlatform.Business.Interfaces;

namespace ThreatIntelligencePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("Signup")]
        public async Task<IActionResult> SignupAsync(SignupDto dto)
        {
            try
            {
                var authResponse = await _authService.SignupAsync(dto);
                return Ok(authResponse);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while signing up.");
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginDto dto)
        {
            try
            {
                var authResponse = await _authService.LoginAsync(dto);
                return Ok(authResponse);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while logging in.");
            }
        }
    }
}
