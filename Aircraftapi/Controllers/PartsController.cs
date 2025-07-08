using Aircraftapi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AircraftDashboardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PartsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetAllParts()
        {
            var parts = await _context.Parts.ToListAsync();
            return Ok(parts);
        }

        [HttpGet("in-progress-count")]
        public async Task<IActionResult> GetInProgressPartsCount()
        {
            var count = await _context.Parts.CountAsync(p => p.Status == "In Progress");
            return Ok(count);
        }

        
    }
}
