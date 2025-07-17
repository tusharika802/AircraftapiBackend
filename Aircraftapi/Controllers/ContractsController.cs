using Aircraftapi.Data;
using Aircraftapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AircraftDashboardAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "admin")]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public ContractsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContracts()
        {
            var allPartners = await _context.Partners.ToListAsync();
            var contracts = await _context.Contracts.ToListAsync();

            var result = contracts.Select(contract =>
            {
                var ids = contract.PartnerIds?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var parsedId) ? parsedId : -1)
                    .Where(id => id != -1)
                    .ToList() ?? new List<int>();

                var partnerNames = allPartners
                    .Where(p => ids.Contains(p.Id))
                    .Select(p => p.Name)
                    .ToList();

                return new
                {
                    contract.Id,
                    contract.Title,
                    contract.IsActive,
                    PartnerIds = contract.PartnerIds ?? "",
                    PartnerNames = string.Join(", ", partnerNames)
                };
            });

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetActiveContractsCount()
        {
            var count = await _context.Contracts.CountAsync(c => c.IsActive);
            return Ok(count);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddContract([FromBody] ContractCreateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Invalid contract data");

            // Convert comma-separated string to List<int>
            var partnerIdList = dto.PartnerIds?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var parsedId) ? parsedId : -1)
                .Where(id => id != -1)
                .ToList() ?? new List<int>();

            if (partnerIdList.Count == 0)
                return BadRequest("No valid partner IDs provided.");

            var validPartners = await _context.Partners
                .Where(p => partnerIdList.Contains(p.Id))
                .ToListAsync();

            if (validPartners.Count != partnerIdList.Count)
                return BadRequest("One or more selected partners do not exist.");

            string partnerIdStr = string.Join(",", validPartners.Select(p => p.Id));

            var contract = new Contract
            {
                Title = dto.Title,
                IsActive = dto.IsActive,
                PartnerIds = partnerIdStr
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            // Email Notification
            var allPartnerNames = validPartners.Select(p => p.Name).ToList();

            foreach (var partner in validPartners)
            {
                if (!string.IsNullOrEmpty(partner.Email))
                {
                    var otherPartners = allPartnerNames
                        .Where(name => name != partner.Name)
                        .ToList();

                    var otherPartnersText = otherPartners.Any()
                        ? string.Join(", ", otherPartners)
                        : "No other partners.";

                    var subject = "New Contract Assignment";

                    var body = new StringBuilder();
                    body.AppendLine($"<p>Dear {partner.Name},</p>");
                    body.AppendLine($"<p>You have been assigned to the contract: <strong>{contract.Title}</strong>.</p>");
                    body.AppendLine($"<p><strong>Other partners:</strong> {otherPartnersText}</p>");
                    body.AppendLine("<p>Thank you.</p>");

                    await _emailSender.SendEmailAsync(partner.Email, subject, body.ToString());
                }
            }

            return Ok(contract);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] Contract updatedContract)
        {
            var existing = await _context.Contracts.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Title = updatedContract.Title;
            existing.IsActive = updatedContract.IsActive;
            existing.PartnerIds = updatedContract.PartnerIds;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("delete/{id}")]
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
