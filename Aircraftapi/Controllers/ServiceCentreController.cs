using Aircraftapi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Aircraftapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceCentreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ServiceCentreController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllServiceCentres()
        {
            var centres = await _context.ServiceCentres.ToListAsync();
            return Ok(centres);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetServiceCentreCount()
        {
            var count = await _context.ServiceCentres.CountAsync();
            return Ok(count);
        }
    }
}
