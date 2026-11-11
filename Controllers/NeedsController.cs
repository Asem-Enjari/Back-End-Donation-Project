using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.Models;
using SchoolDonation.API.DTOs;
using System.Security.Claims;

namespace SchoolDonation.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class NeedsController : ControllerBase
	{
		private readonly SchoolDonationContext _context;

		public NeedsController(SchoolDonationContext context)
		{
			_context = context;
		}

		
		[HttpGet("Completed")]
		public async Task<ActionResult<IEnumerable<Need>>> GetCompletedNeeds()
		{
			var completedNeeds = await _context.Needs
											   .Include(n => n.School)
											   .Where(n => n.Status == "Completed")
											   .ToListAsync();

			return Ok(completedNeeds);
		}

		
		[HttpPost]
		public async Task<ActionResult> CreateNeed([FromBody] CreateNeedDto dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			// 🔹 1- جلب ManagerId من الجلسة (Claims)
			int managerIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			if (managerIdClaim == null)
				return Unauthorized("ManagerId not found in token.");


			// 🔹 2- البحث عن المدرسة الخاصة بالمدير
			var school = await _context.Schools
									   .FirstOrDefaultAsync(s => s.ManagerId == managerIdClaim);

			if (school == null)
				return NotFound("No school found for this manager.");

			// 🔹 3- إنشاء الاحتياج
			var need = new Need
			{
				Title = dto.Title,
				Description = dto.Description,
				Cost = dto.Cost,

				CollectedAmount = 0,
				Status = "Pending",
				PublishDate = DateTime.Now,
				IsApproved = false,

				SchoolId = school.SchoolId
			};

			_context.Needs.Add(need);
			await _context.SaveChangesAsync();

			return Ok(new
			{
				message = "Need created successfully",
				needId = need.NeedId
			});
		}
	}
}
