using Aircraftapi.Data;
using Aircraftapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AircraftDashboardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "admin")]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }


        //[HttpGet]
        //public async Task<IActionResult> GetAllContracts()
        //{
        //    var contracts = await _context.Contracts.Include(x => x.ContractPartners).ToListAsync();
        //    var gg = contracts.Select(x => new
        //    {
        //        x.Id,
        //        x.Title,
        //        x.IsActive,
        //        ContractPartner = x.ContractPartners.Select(y => new  { y.Id, y.Name, y.ContractId}).ToList()
        //    });

        //    return Ok(gg);
        //}
      
        [HttpGet]
        public async Task<IActionResult> GetAllContracts()
        {
            var contracts = await _context.Contracts
                .Include(x => x.ContractPartners)
                .ToListAsync();

            var result = contracts.Select(x => new
            {
                x.Id,
                x.Title,
                x.IsActive,
                ContractPartner = x.ContractPartners.Any()
                    ? string.Join(", ", x.ContractPartners.Select(p => p.Name))
                    : string.Empty
            });

            return Ok(result);
        }


        [HttpGet("count")]
        public async Task<IActionResult> GetActiveContractsCount()
        {
            var count = await _context.Contracts.CountAsync(c => c.IsActive);
            return Ok(count);
        }
        // ➕ Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddContract([FromBody] Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return Ok(contract);
        }

        // ✏️ Edit
        [HttpPut("edit")]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] Contract contract)
        {
            var existing = await _context.Contracts.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Title = contract.Title;
            existing.IsActive = contract.IsActive;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ❌ Delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
