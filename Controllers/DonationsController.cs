using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.Models;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Donor")] // <--- Only Donors can donate
    public class DonationsController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        public DonationsController(SchoolDonationContext context)
        {
            _context = context;
        }

        // POST: api/Donations
        [HttpPost]
        public async Task<IActionResult> MakeDonation(int needId, decimal amount)
        {
            if (amount <= 0) return BadRequest("Amount must be greater than 0.");

            // 1. Get the Need (Locking logic would go here in a real bank app)
            var need = await _context.Needs.FindAsync(needId);
            if (need == null) return NotFound("Project not found.");

            // 2. Check if already completed
            if (need.Status == "Completed") 
                return BadRequest("This project is already fully funded!");

            // 3. Check if amount is too much
            decimal remaining = need.Cost - (need.CollectedAmount ?? 0);
            if (amount > remaining)
            {
                return BadRequest($"You cannot donate more than the remaining amount: {remaining}");
            }

            // 4. Record the Donation
            int donorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            var donation = new Donation
            {
                DonorId = donorId,
                NeedId = needId,
                Amount = amount,
                DonationDate = DateTime.Now,
                TransactionReference = Guid.NewGuid().ToString() // Fake Bank ID
            };

            _context.Donations.Add(donation);

            // 5. Update the Need (Add Money)
            need.CollectedAmount = (need.CollectedAmount ?? 0) + amount;

            // 6. Check if Finished
            if (need.CollectedAmount >= need.Cost)
            {
                need.Status = "Completed"; // Mark as done!
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Thank you for your donation!", NewBalance = need.CollectedAmount, Status = need.Status });
        }

        // GET: api/Donations/MyHistory
        [HttpGet("MyHistory")]
        public async Task<IActionResult> GetMyHistory()
        {
            int donorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var history = await _context.Donations
                .Include(d => d.Need) // Include Need to show title
                .ThenInclude(n => n.School) // Include School to show name
                .Where(d => d.DonorId == donorId)
                .Select(d => new 
                {
                    d.DonationId,
                    SchoolName = d.Need.School.Name,
                    ProjectTitle = d.Need.Title,
                    AmountDonated = d.Amount,
                    Date = d.DonationDate
                })
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            return Ok(history);
        }
    }
}