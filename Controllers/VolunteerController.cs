using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;
using SchoolDonation.API.Models;
using System.Security.Claims;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteerController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        public VolunteerController(SchoolDonationContext context)
        {
            _context = context;
        }

        // GET: api/Volunteer/AvailableTasks
        [HttpGet("AvailableTasks")]
        public async Task<ActionResult<List<VolunteerActivity>>> GetTasks()
        {
            // Show tasks that are still "Pending" (Waiting for people)
            return await _context.VolunteerActivities
                                 .Where(v => v.Status == "Pending")
                                 .ToListAsync();
        }

        // POST: api/Volunteer/Apply/5
        [Authorize(Roles = "Volunteer")] // <--- ONLY VOLUNTEERS
        [HttpPost("Apply/{activityId}")]
        public async Task<IActionResult> ApplyForTask(int activityId)
        {
            // Logic: In a real app, we would have a table "VolunteerApplications".
            // Since we didn't design that table in the ERD, we will assume 
            // taking the task assigns it to this user immediately.
            
            var task = await _context.VolunteerActivities.FindAsync(activityId);
            if (task == null) return NotFound();

            if (task.Status != "Pending") return BadRequest("Task is already taken.");

            // Assign the task to this volunteer
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            task.VolunteerId = userId;
            task.Status = "Assigned"; // Change status

            await _context.SaveChangesAsync();
            return Ok("You have successfully applied for this task.");
        }







		[Authorize(Roles = "Donor")]
		[HttpPost("AddActivity")]
		public async Task<IActionResult> AddVolunteerActivity([FromBody] CreateVolunteerActivityDto request)
		{
			int volunteerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var activity = new VolunteerActivity
			{
				VolunteerId = volunteerId,
				SchoolId = null,
				Status = "Pending",
				ActivityType = request.Title,   
				Description = request.Description,
				ActivityDate = DateTime.Now
			};

			_context.VolunteerActivities.Add(activity);
			await _context.SaveChangesAsync();

			return Ok(new
			{
				Message = "Volunteer activity created successfully.",
				ActivityId = activity.ActivityId,
				ActivityType = activity.ActivityType,
				Status = activity.Status,
				Date = activity.ActivityDate
			});
		}


		// GET: api/Volunteer/PhysicalActivities
		[HttpGet("PhysicalActivities")]
		public async Task<ActionResult<List<VolunteerActivity>>> GetPhysicalActivities()
		{
			return await _context.VolunteerActivities
								 .Where(v => v.ActivityType == "Physical")
								 .Include(v => v.Volunteer)
								 .ToListAsync();
		}


		// GET: api/Volunteer/DigitalActivities
		[HttpGet("DigitalActivities")]
		public async Task<ActionResult<List<VolunteerActivity>>> GetDigitalActivities()
		{
			return await _context.VolunteerActivities
								 .Where(v => v.ActivityType == "Digital")
								 .Include(v => v.Volunteer)
								 .ToListAsync();
		}

	}
}