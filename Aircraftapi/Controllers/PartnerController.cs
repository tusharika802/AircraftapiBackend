using Aircraftapi.Data;
using Aircraftapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aircraftapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "admin")]

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
        //// ➕ Add partner
        //[HttpPost("add")]
        //public async Task<IActionResult> AddPartner([FromBody] Partner partner)
        //{
        //    _context.Partners.Add(partner);
        //    await _context.SaveChangesAsync();
        //    return Ok(partner);
        //}

        //[HttpPost("add")]
        //public async Task<IActionResult> AddPartner([FromBody] Partner partner)
        //{
        //    // If contractId is 0 or invalid, treat it as null
        //    if (partner.ContractId == 0)
        //        partner.ContractId = null;

        //    _context.Partners.Add(partner);
        //    await _context.SaveChangesAsync();

        //    var result = await _context.Partners
        //        .Include(p => p.Contract)
        //        .FirstOrDefaultAsync(p => p.Id == partner.Id);

        //    return Ok(result);
        //}
        [HttpPost("add")]
        public async Task<IActionResult> AddPartner([FromBody] Partner partner)
        {
            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();
            return Ok(partner);
        }


        // ✏️ Edit partner
        [HttpPut("edit")]
        public async Task<IActionResult> EditPartner(int id, [FromBody] Partner partner)
        {
            var existing = await _context.Partners.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = partner.Name;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ❌ Delete partner
        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
                return NotFound();

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }


}
