using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.Models;
using SchoolDonation.API.Services;
using SchoolDonation.API.DTOs;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")] // <--- ONLY MANAGERS
    public class ManagerController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        private readonly FileService _fileService;
        public ManagerController(SchoolDonationContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // Helper to get the logged-in user's ID
        private int GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!.Value);
        }

        // POST: api/Manager/CreateSchool
        [HttpPost("CreateSchool")]
        public async Task<IActionResult> CreateSchoolProfile(School school)
        {
            int userId = GetCurrentUserId();

            // Check if this manager already has a school
            if (_context.Schools.Any(s => s.ManagerId == userId))
                return BadRequest("You already have a school profile.");

            // Force the ManagerId to be the logged-in user (Security!)
            school.ManagerId = userId;

            _context.Schools.Add(school);
            await _context.SaveChangesAsync();

            return Ok(school);
        }

        // POST: api/Manager/AddNeed
        [HttpPost("AddNeed")]
        public async Task<IActionResult> AddNeed(Need need)
        {
            int userId = GetCurrentUserId();

            // 1. Find the school belonging to this manager
            var school = await _context.Schools.FirstOrDefaultAsync(s => s.ManagerId == userId);
            if (school == null) return BadRequest("You must create a school profile first.");

            // 2. Link Need to School
            need.SchoolId = school.SchoolId;
            need.Status = "Pending";
            need.CollectedAmount = 0;

            _context.Needs.Add(need);
            await _context.SaveChangesAsync();

            return Ok(need);
        }
        // POST: api/Manager/UploadImage
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] SchoolDonation.API.DTOs.ImageUploadDto request)
        {
            // 1. Verify User is a Manager
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // 2. Verify the Need belongs to this Manager's school
            var need = await _context.Needs
                .Include(n => n.School)
                .FirstOrDefaultAsync(n => n.NeedId == request.NeedId); // <--- Use request.NeedId

            if (need == null) return NotFound("Project not found.");
            if (need.School.ManagerId != userId) return Unauthorized("This project does not belong to your school.");

            // 3. Upload the file
            if (request.File == null || request.File.Length == 0) return BadRequest("No file uploaded.");

            string imagePath = await _fileService.SaveImageAsync(request.File); // <--- Use request.File

            // 4. Save record in Database
            var image = new Image
            {
                NeedId = request.NeedId,
                ImageUrl = imagePath,
                ImageType = request.ImageType, // <--- Use request.ImageType
                UploadDate = DateTime.Now
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Image uploaded successfully", Path = imagePath });
        }
        // POST: api/Manager/AssignStudent
        [HttpPost("AssignStudent")]
        public async Task<IActionResult> AssignStudent(AssignStudentDto request)
        {
            // 1. Find Sponsorship
            var s = await _context.StudentSponsorships
                .Include(x => x.School)
                .FirstOrDefaultAsync(x => x.SponsorId == request.SponsorId);

            if (s == null) return NotFound();

            // 2. Verify it belongs to Manager's School
            int managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (s.School.ManagerId != managerId) return Unauthorized();

            // 3. Fill Student Data
            s.StudentFullName = request.StudentFullName;
            s.StudentGrade = request.StudentGrade;
            s.SponsorshipReason = request.Reason;

            // 4. Update Status
            s.Status = "Pending Admin Review";
            s.IsApproved = false;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Student assigned. Sent to Admin for approval." });
        }
    }
}