using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using ThreatIntelligencePlatform.Business.Interfaces;
using ThreatIntelligencePlatform.Business.Services;

namespace ThreatIntelligencePlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IoCController : ControllerBase
    {
        private readonly IIoCService _iocService;
        
        public IoCController(IIoCService iocService) 
        {
            _iocService = iocService;
        }
        
        [HttpGet("GetAll")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetAllAsync([FromQuery] long limit, [FromQuery] long offset,
            [FromQuery] string? search)
        {
            var iocs = await _iocService.LoadAsync(limit, offset, search);
            return Ok(iocs);
        }

        [HttpGet("Count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountAsync()
        {
            var count = await _iocService.CountAsync();
            return Ok(count);
        }

        [HttpGet("CountByType")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountByTypeAsync()
        {
            var counts = await _iocService.CountByTypeAsync();
            return Ok(counts);
        }

        [HttpGet("CountSpecificType")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountSpecificTypeAsync([FromQuery] string type)
        {
            var count = await _iocService.CountSpecificTypeAsync(type);
            return Ok(count);
        }

        [HttpGet("CountBySource")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountBySourceAsync()
        {
            var counts = await _iocService.CountBySourceAsync();
            return Ok(counts);
        }

        [HttpGet("CountSpecificSource")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountSpecificSourceAsync([FromQuery] string source)
        {
            var count = await _iocService.CountSpecificSourceAsync(source);
            return Ok(count);
        }

        [HttpGet("CountTypesBySource")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountTypesBySourceAsync()
        {
            var counts = await _iocService.CountTypesBySourceAsync();
            return Ok(counts);
        }

        [HttpGet("CountBySourceAndType")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountBySourceAndTypeAsync([FromQuery] string source)
        {
            var counts = await _iocService.CountBySourceAndTypeAsync(source);
            return Ok(counts);
        }

        [HttpGet("CountByTypeAndSource")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CountByTypeAndSourceAsync([FromQuery] string type)
        {
            var counts = await _iocService.CountByTypeAndSourceAsync(type);
            return Ok(counts);
        }
    }
}
