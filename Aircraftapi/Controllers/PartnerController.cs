using Aircraftapi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aircraftapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PartnerController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPartners()
        {
            var partners = await _context.Partners.ToListAsync();
            return Ok(partners);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetPartnerCount()
        {
            var count = await _context.Partners.CountAsync();
            return Ok(count);
        }

    }
}
