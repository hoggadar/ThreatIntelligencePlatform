using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAllAsync([FromQuery] long limit, [FromQuery] long offset,
            [FromQuery] string search)
        {
            var iocs = await _iocService.LoadAsync(limit, offset);
            return Ok(iocs);
        }
    }
}
