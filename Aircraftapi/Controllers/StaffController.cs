using Aircraftapi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aircraftapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
       private readonly ApplicationDbContext _context;
        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _context.Staffs.ToListAsync();
            return Ok(staff);
        }

        [HttpGet("active-count")]
        public async Task<IActionResult> GetActiveStaffCount()
        {
            var count = await _context.Staffs.CountAsync(s => s.IsActive);
            return Ok(count);
        }

    }
}
