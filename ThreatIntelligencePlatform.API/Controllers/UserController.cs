using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.Role;
using ThreatIntelligencePlatform.Business.DTOs.User;
using ThreatIntelligencePlatform.Business.Interfaces;

namespace ThreatIntelligencePlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("GetAll")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetAllAsync([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        if (pageIndex < 1)
            return BadRequest("Page index must be greater than 0");
        
        if (pageSize < 1)
            return BadRequest("Page size must be greater than 0");

        try
        {
            var users = await _userService.GetAllAsync(pageIndex, pageSize);
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving users.");
        }
    }

    [HttpGet("GetById/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("User ID cannot be empty.");

        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID '{id}' was not found.");

            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving the user.");
        }
    }

    [HttpGet("GetByEmail/{email}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetByEmailAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest("Email cannot be empty.");

        try
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound($"User with email '{email}' was not found.");

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving the user.");
        }
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> CreateAsync([FromBody] CreateUserDto dto)
    {
        if (dto == null)
            return BadRequest("User data cannot be null.");

        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest("Email cannot be empty.");

        if (string.IsNullOrEmpty(dto.Password))
            return BadRequest("Password cannot be empty.");

        try
        {
            var user = await _userService.CreateAsync(dto);
            return StatusCode(201, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while creating the user.");
        }
    }

    [HttpPut("Update/{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateAsync(string id, [FromBody] UpdateUserDto dto)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("User ID cannot be empty.");

        if (dto == null)
            return BadRequest("User data cannot be null.");

        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest("Email cannot be empty.");

        try
        {
            var user = await _userService.UpdateAsync(id, dto);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating the user.");
        }
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("User ID cannot be empty.");

        try
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }

    [HttpGet("GetUserRoles/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IList<string>>> GetUserRolesAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest("User ID cannot be empty.");

        try
        {
            var roles = await _userService.GetUserRolesAsync(id);
            return Ok(roles);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving user roles.");
        }
    }

    [HttpPost("AddToRole")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AddToRoleAsync([FromBody] UserRoleDto dto)
    {
        if (dto == null)
            return BadRequest("Role data cannot be null.");

        if (string.IsNullOrEmpty(dto.UserId))
            return BadRequest("User ID cannot be empty.");

        if (string.IsNullOrEmpty(dto.Role))
            return BadRequest("Role cannot be empty.");

        try
        {
            await _userService.AddToRoleAsync(dto.UserId, dto.Role);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding role to user.");
        }
    }

    [HttpPost("RemoveFromRole")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RemoveFromRoleAsync([FromBody] UserRoleDto dto)
    {
        if (dto == null)
            return BadRequest("Role data cannot be null.");

        if (string.IsNullOrEmpty(dto.UserId))
            return BadRequest("User ID cannot be empty.");

        if (string.IsNullOrEmpty(dto.Role))
            return BadRequest("Role cannot be empty.");

        try
        {
            await _userService.RemoveFromRoleAsync(dto.UserId, dto.Role);
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while removing role from user.");
        }
    }
}
