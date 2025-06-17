using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ThreatIntelligencePlatform.Business.DTOs.Pagination;
using ThreatIntelligencePlatform.Business.DTOs.Role;
using ThreatIntelligencePlatform.Business.Interfaces;

namespace ThreatIntelligencePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<PaginatedList<RoleDto>>> GetAllAsync([FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageIndex < 1)
                return BadRequest("Page index must be greater than 0");
            
            if (pageSize < 1)
                return BadRequest("Page size must be greater than 0");
            
            

            try
            {
                var roles = await _roleService.GetAllAsync(pageIndex, pageSize);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving roles.");
            }
        }

        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<RoleDto>> GetByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Role name cannot be empty.");

            try
            {
                var role = await _roleService.GetByNameAsync(name);
                if (role == null)
                    return NotFound($"Role with name '{name}' was not found.");

                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the role.");
            }
        }

        [HttpGet("GetUserRoles/{userId}")]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetUserRolesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID cannot be empty.");

            try
            {
                var roles = await _roleService.GetUserRolesAsync(userId);
                return Ok(roles);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving user roles.");
            }
        }
    }
}
