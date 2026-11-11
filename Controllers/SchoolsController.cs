using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SchoolDonation.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class SchoolsController : ControllerBase
	{
		private readonly SchoolDonationContext _context;

		public SchoolsController(SchoolDonationContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetSchools()
		{
			// This line tests the connection
			var schools = await _context.Schools.ToListAsync();
			if (schools == null)
			{
				return Ok("no schools found ");
			}
			return Ok(schools);
		}


        [HttpGet("MySchool")]
        public async Task<IActionResult> GetMySchool()
        {
            // 1️⃣ Get UserId from JWT
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // 2️⃣ Get Manager + School + Needs
            var manager = await _context.Managers
                .Include(m => m.School)
                    .ThenInclude(s => s.Needs)
                .FirstOrDefaultAsync(m => m.ManagerNavigation.UserId == userId);

            if (manager == null)
                return NotFound("Manager not found.");

            if (manager.School == null)
                return NotFound("No school assigned to this manager.");

            var school = manager.School;

            // 3️⃣ Return School + Needs
            return Ok(new
            {
                school.SchoolId,
                school.ManagerId,
                school.Name,
                school.Capacity,
                school.Status,
                school.Governorate,
                school.City,
                school.Street,

                Needs = school.Needs.Select(n => new
                {
                    n.NeedId,
                    n.Title,
                    n.Description,
                    n.Cost,
                    n.CollectedAmount,
                    n.Status,
                    n.PublishDate,
                    n.IsApproved
                })
            });
        }
    }
}