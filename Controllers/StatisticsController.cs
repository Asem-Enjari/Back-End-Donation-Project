using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;
using SchoolDonation.API.Models;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly SchoolDonationContext _context;

    public StatisticsController(SchoolDonationContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatistics()
    {
        var supportedSchools = await _context.Schools
            .Where(s => s.Status == "Supported" || s.Needs.Any())
            .CountAsync();

        var donors = await _context.Users
            .Where(u => u.UserRole == "Donor")
            .CountAsync();

        var totalDonations = await _context.Donations
            .SumAsync(d => (decimal?)d.Amount) ?? 0;

        var volunteers = await _context.VolunteerActivities
            .CountAsync();

        var result = new StatisticsDto
        {
            SupportedSchools = supportedSchools,
            Donors = donors,
            TotalDonations = totalDonations,
            Volunteers = volunteers
        };

        return Ok(result);
    }

    [HttpGet("ActiveNeeds")]
	public async Task<ActionResult<int>> GetActiveNeedsCount()
	{
		var count = await _context.Needs
								  .Where(n => n.Status == "Active")
								  .CountAsync();

		return Ok(count);
	}
}
