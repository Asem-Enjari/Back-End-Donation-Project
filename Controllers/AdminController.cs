using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;
using SchoolDonation.API.Models;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // <--- ONLY ADMIN CAN ENTER HERE
    public class AdminController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        public AdminController(SchoolDonationContext context)
        {
            _context = context;
        }

        // POST: api/Admin/CreateManager

        [HttpPost("CreateManager")]
        public async Task<IActionResult> CreateManager(UserDto request)
        {
            // 1. Validation
            if (_context.Users.Any(u => u.Email == request.Email))
                return BadRequest("Email exists.");

            // 2. Create User
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                UserRole = "Manager" // FORCE ROLE TO MANAGER
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. Create Manager Profile
            var manager = new Manager
            {
                ManagerId = user.UserId,
                EmployeeNumber = "EMP-" + Guid.NewGuid().ToString().Substring(0, 8)
            };
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Manager created successfully", Id = user.UserId });
        }
        // GET: api/Admin/PendingSponsorships

        [HttpGet("PendingSponsorships")]
        public async Task<ActionResult<List<SponsorshipViewDto>>> GetPendingSponsorships()
        {
            var list = await _context.StudentSponsorships
                .Include(s => s.Donor)
                .Where(s => s.Status == "Pending Admin Review" && s.IsApproved == false)
                .Select(s => new SponsorshipViewDto
                {
                    SponsorId = s.SponsorId,
                    DonorName = s.IsAnonymous == true ? "Anonymous" : s.Donor.FirstName + " " + s.Donor.LastName,
                    Amount = s.Amount,
                    Description = s.Description,
                    ItemType = s.ItemType,
                    Date = s.SponsorshipDate ?? DateTime.Now,
                    Status = s.Status,
                    IsApproved = s.IsApproved,
                    StudentFullName = s.StudentFullName,
                    StudentGrade = s.StudentGrade ?? 0
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST: api/Admin/ApproveSponsorship/5

        [HttpPost("ApproveSponsorship/{id}")]
        public async Task<IActionResult> ApproveSponsorship(int id)
        {
            var s = await _context.StudentSponsorships.FindAsync(id);
            if (s == null) return NotFound();

            s.IsApproved = true;
            s.Status = "Completed"; // Final Success State
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Sponsorship Approved and Verified." });
        }
        // 1. GET: api/Admin/PendingNeeds
        // Returns a list of Needs waiting for approval
        [HttpGet("PendingNeeds")]
        public async Task<ActionResult> GetPendingNeeds()
        {
            var pending = await _context.Needs
                .Include(n => n.School) // Include School to see the name
                .Where(n => n.IsApproved == false) // <--- Only fetch unapproved
                .Select(n => new
                {
                    n.NeedId,
                    SchoolName = n.School.Name,
                    n.Title,
                    n.Description,
                    n.Cost,
                    n.PublishDate
                })
                .ToListAsync();

            return Ok(pending);
        }

        // 2. POST: api/Admin/ApproveNeed/5
        // The switch that makes it visible on the website
        [HttpPost("ApproveNeed/{needId}")]
        public async Task<IActionResult> ApproveNeed(int needId)
        {
            var need = await _context.Needs.FindAsync(needId);
            if (need == null) return NotFound();

            // Change the flag
            need.IsApproved = true;
            need.Status = "Active";


			await _context.SaveChangesAsync();

            return Ok(new { Message = "Project approved successfully. It is now visible to donors." });
        }
    }
}