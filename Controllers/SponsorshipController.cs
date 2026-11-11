using System.Security.Claims;
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
	public class SponsorshipController : ControllerBase
	{
		private readonly SchoolDonationContext _context;

		public SponsorshipController(SchoolDonationContext context)
		{
			_context = context;
		}

		// =========================
		// Helper Method
		// =========================
		private EducationLevel GetLevelFromGrade(int grade)
		{
			if (grade >= 1 && grade <= 6) return EducationLevel.Basic;
			if (grade >= 7 && grade <= 9) return EducationLevel.Preparatory;
			if (grade >= 10 && grade <= 12) return EducationLevel.Secondary;

			return EducationLevel.Basic;
		}

		// =====================================================
		// 1️⃣ POST: api/Sponsorship
		// Donor creates sponsorship → Pending Assignment
		// =====================================================
		[HttpPost]
		public async Task<IActionResult> CreateSponsorship(CreateSponsorshipDto request)
		{
			int UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var school = _context.Schools.FirstOrDefault(s => s.ManagerId == UserID);

            var sponsorship = new StudentSponsorship
			{
				//DonorId = donorId,
				SchoolId = school.SchoolId ,
				ItemType = request.ItemType,
				Amount = request.Amount,
				Description = request.Description,
				IsAnonymous = true,
				SponsorshipDate = DateTime.Now,

				Status = "Pending", 
				IsApproved = false
			};

			_context.StudentSponsorships.Add(sponsorship);
			await _context.SaveChangesAsync();

			return Ok(new
			{
				Message = "Sponsorship recorded successfully. Waiting for manager to assign a student."
			});
		}

		// =====================================================
		// 2️⃣ GET: api/Sponsorship/pending-assignment
		// Admin / Manager → View pending sponsorships
		// =====================================================
		[HttpGet("pending")]
		public async Task<IActionResult> GetPendingAssignments()
		{
			var pending = await _context.StudentSponsorships
				.Include(s => s.School)
				.Where(s => s.Status == "Pending")
				.OrderByDescending(s => s.SponsorshipDate)
				.ToListAsync();

			return Ok(pending);
		}

		// =====================================================
		// 3️⃣ GET: api/Sponsorship/Public
		// Public → Only approved & completed sponsorships
		// =====================================================
		[HttpGet("Public")]
		public async Task<ActionResult<List<PublicSponsorshipDto>>> GetPublicSponsorships(
			[FromQuery] EducationLevel? level)
		{
			var allSponsorships = await _context.StudentSponsorships
				.Include(s => s.School)
				.Where(s => s.IsApproved == true && s.Status == "Completed")
				.ToListAsync();

			var resultList = new List<PublicSponsorshipDto>();

			foreach (var s in allSponsorships)
			{
				if (s.StudentGrade == null)
					continue;

				var currentLevel = GetLevelFromGrade(s.StudentGrade.Value);

				if (level.HasValue && currentLevel != level.Value)
					continue;

				resultList.Add(new PublicSponsorshipDto
				{
					SponsorId = s.SponsorId,
					SchoolName = s.School.Name,
					StudentGrade = s.StudentGrade.Value,
					EducationLevel = currentLevel.ToString(),
					ItemType = s.ItemType,
					Amount = s.Amount,
					Description = s.Description,
					Date = s.SponsorshipDate ?? DateTime.Now
				});
			}

			return Ok(resultList);
		}
	}
}
