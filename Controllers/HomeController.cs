using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        public HomeController(SchoolDonationContext context)
        {
            _context = context;
        }

        // 1. GET: api/Home/Statistics
        // Returns the top 3 numbers (Total Schools, Money, Active Projects)
        [HttpGet("Statistics")]
        public async Task<ActionResult<HomeStatsDto>> GetStatistics()
        {
            var stats = new HomeStatsDto
            {
                // Count all schools
                TotalSchools = await _context.Schools.CountAsync(),

                // Sum of all money collected in Needs
                TotalCollectedAmount = await _context.Needs
    .Where(n => n.IsApproved == true)
    .SumAsync(n => n.CollectedAmount ?? 0),

                // Count needs where Status is NOT 'Completed'
                ActiveProjects = await _context.Needs.CountAsync(n => n.Status != "Completed" && n.IsApproved == true)
            };

            return Ok(stats);
        }

        // 2. GET: api/Home/LatestProjects
        // Returns the list of cards
        [HttpGet("LatestProjects")]
        public async Task<ActionResult<List<SchoolCardDto>>> GetLatestProjects()
        {
            var schools = await _context.Schools
                .Include(s => s.Needs) // We need Needs to calculate money and count items
                .Select(s => new SchoolCardDto
                {
                    SchoolId = s.SchoolId,
                    Name = s.Name,
                    Status = s.Status!,
                    City = s.Governorate + " - " + s.City, // e.g., "Aleppo - Center"
                    Capacity = s.Capacity ?? 0,
                    NeedsCount = s.Needs.Count(n => n.Status != "Completed" && n.IsApproved == true),

                    // Logic to find the main image (or a placeholder)
                    ImageUrl = s.Needs.SelectMany(n => n.Images)
                  .Where(img => img.ImageType == "Before" || img.ImageType == "During")
                  .Select(img => img.ImageUrl)
                  .FirstOrDefault()
                  ?? "https://via.placeholder.com/300", // We will fix this later with real images

                    // THE MATH FOR THE GREEN BAR:
                    // (Sum of Collected Money / Sum of Total Cost) * 100
                    // THE MATH FOR THE GREEN BAR (Only count Approved items):
                    ProgressPercentage = s.Needs.Where(n => n.IsApproved == true).Sum(n => n.Cost) == 0
    ? 0
    : (int)((s.Needs.Where(n => n.IsApproved == true).Sum(n => n.CollectedAmount ?? 0)
             / s.Needs.Where(n => n.IsApproved == true).Sum(n => n.Cost)) * 100)
                })
                .OrderByDescending(s => s.SchoolId) // Show newest first
                .Take(6) // Only show top 6 on home page
                .ToListAsync();

            return Ok(schools);
        }
        [HttpGet("RecentDonations")]
        public async Task<ActionResult<List<RecentDonationDto>>> GetRecentDonations()
        {
            var donations = await _context.Donations
                .Include(d => d.Donor) // We need the user to get the name
                .OrderByDescending(d => d.DonationDate)
                .Take(5) // Only show the last 5
                .ToListAsync();

            var result = donations.Select(d => new RecentDonationDto
            {
                // Logic: If we had an 'IsAnonymous' flag in Donations/User, we would check it here.
                // For now, let's assume if Firstname is "Guest", we hide it.
                // In a real scenario: d.IsAnonymous ? "فاعل خير" : d.User.FirstName
                DonorName = d.Donor.FirstName + " " + d.Donor.LastName.Substring(0, 1) + ".", // "Ahmad M."

                Amount = d.Amount,

                // Helper to make time look nice (Social Media style)
                TimeAgo = GetTimeAgo(d.DonationDate ?? DateTime.Now)
            }).ToList();

            return Ok(result);
        }

        // 4. GET: api/Home/Search?query=Aleppo
        // Feeds the Top Search Bar
        [HttpGet("Search")]
        public async Task<ActionResult<List<SearchResultDto>>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Ok(new List<SearchResultDto>());

            var results = new List<SearchResultDto>();

            // A. Search Schools
            var schools = await _context.Schools
                .Where(s => s.Name.Contains(query) || s.City.Contains(query))
                .Select(s => new SearchResultDto
                {
                    Id = s.SchoolId,
                    Type = "School",
                    Title = s.Name,
                    Subtitle = s.City
                })
                .Take(5)
                .ToListAsync();
            results.AddRange(schools);

            // B. Search Projects (Needs)
            var needs = await _context.Needs
                .Include(n => n.School)
                .Where(n => n.Title.Contains(query) && n.IsApproved == true)
                .Select(n => new SearchResultDto
                {
                    Id = n.NeedId, // Note: You might want to navigate to the School ID instead
                    Type = "Project",
                    Title = n.Title,
                    Subtitle = n.School.Name
                })
                .Take(5)
                .ToListAsync();
            results.AddRange(needs);

            return Ok(results);
        }

        // Helper Function for "Time Ago"
        private static string GetTimeAgo(DateTime date)
        {
            var span = DateTime.Now - date;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} h ago";
            return $"{(int)span.TotalDays} d ago";
        }
    }
}