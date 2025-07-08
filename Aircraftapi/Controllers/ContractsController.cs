using Aircraftapi.Data;
using Aircraftapi.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AircraftDashboardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContracts()
        {
            var contracts = await _context.Contracts.ToListAsync();
            return Ok(contracts);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetActiveContractsCount()
        {
            var count = await _context.Contracts.CountAsync(c => c.IsActive);
            return Ok(count);
        }

       
    }
}
