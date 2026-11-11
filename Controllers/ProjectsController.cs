using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        public ProjectsController(SchoolDonationContext context)
        {
            _context = context;
        }

        // GET: api/Projects?governorate=Aleppo
        [HttpGet]
        public async Task<ActionResult<List<SchoolCardDto>>> GetAllProjects([FromQuery] string? governorate)
        {
            var query = _context.Schools
                .Include(s => s.Needs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(governorate))
            {
                query = query.Where(s => s.Governorate == governorate);
            }

            var projects = await query
                .Select(s => new SchoolCardDto
                {
                    SchoolId = s.SchoolId,
                    Name = s.Name,
                    Status = s.Status!,
                    City = s.Governorate + " - " + s.City,
                    Capacity = s.Capacity ?? 0,
                    
                    // EDIT 1: Only count needs that are Approved AND not completed
                    NeedsCount = s.Needs.Count(n => n.Status != "Completed" && n.IsApproved == true),
                    
                    // Logic to find real image (from previous steps) or keep placeholder
                    ImageUrl = s.Needs.SelectMany(n => n.Images)
                                      .Where(img => img.ImageType == "Before")
                                      .Select(img => img.ImageUrl)
                                      .FirstOrDefault() ?? "https://via.placeholder.com/300", 

                    // EDIT 2: Update the Math to Ignore Unapproved Costs
                    ProgressPercentage = s.Needs.Where(n => n.IsApproved == true).Sum(n => n.Cost) == 0 
                        ? 0 
                        : (int)((s.Needs.Where(n => n.IsApproved == true).Sum(n => n.CollectedAmount ?? 0) 
                                 / s.Needs.Where(n => n.IsApproved == true).Sum(n => n.Cost)) * 100)
                })
                .ToListAsync();

            return Ok(projects);
        }

        // GET: api/Projects/5
        [HttpGet("{schoolId}")]
        public async Task<IActionResult> GetProjectDetails(int schoolId)
        {
            var school = await _context.Schools
                .Include(s => s.Needs) 
                .Include(s=>s.SchoolLocation)
                .FirstOrDefaultAsync(s => s.SchoolId == schoolId);

            // EDIT 3: Check for NULL first, THEN filter
            if (school == null) return NotFound();

            // Filter the list in memory so the user only sees Approved items
            school.Needs = school.Needs.Where(n => n.IsApproved == true).ToList();
            
            return Ok(school);
        }
    }
}









// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using SchoolDonation.API.Data;
// using SchoolDonation.API.DTOs;

// namespace SchoolDonation.API.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class ProjectsController : ControllerBase
//     {
//         private readonly SchoolDonationContext _context;

//         public ProjectsController(SchoolDonationContext context)
//         {
//             _context = context;
//         }

//         // GET: api/Projects?governorate=Aleppo
//         [HttpGet]
//         public async Task<ActionResult<List<SchoolCardDto>>> GetAllProjects([FromQuery] string? governorate)
//         {
//             var query = _context.Schools
//                 .Include(s => s.Needs)
//                 .AsQueryable();

//             if (!string.IsNullOrEmpty(governorate))
//             {
//                 query = query.Where(s => s.Governorate == governorate);
//             }

//             var projects = await query
//                 .Select(s => new SchoolCardDto
//                 {
//                     SchoolId = s.SchoolId,
//                     Name = s.Name,
//                     Status = s.Status!,
//                     City = s.Governorate + " - " + s.City,
//                     Capacity = s.Capacity ?? 0,
                    
//                     // EDIT 1: Only count needs that are Approved AND not completed
//                     NeedsCount = s.Needs.Count(n => n.Status != "Completed" && n.IsApproved == true),
                    
//                     // Logic to find real image (from previous steps) or keep placeholder
//                     ImageUrl = s.Needs.SelectMany(n => n.Images)
//                                       .Where(img => img.ImageType == "Before")
//                                       .Select(img => img.ImageUrl)
//                                       .FirstOrDefault() ?? "https://via.placeholder.com/300", 

//                     // EDIT 2: Update the Math to Ignore Unapproved Costs
//                     ProgressPercentage = s.Needs.Where(n => n.IsApproved == true).Sum(n => n.Cost) == 0 
//                         ? 0 
//                         : (int)((s.Needs.Where(n => n.IsApproved == true).Sum(n => n.CollectedAmount ?? 0) 
//                                  / s.Needs.Where(n => n.IsApproved == true).Sum(n => n.Cost)) * 100)
//                 })
//                 .ToListAsync();

//             return Ok(projects);
//         }

//         // GET: api/Projects/5
//         [HttpGet("{schoolId}")]
//         public async Task<IActionResult> GetProjectDetails(int schoolId)
//         {
//             var school = await _context.Schools
//                 .Include(s => s.Needs) 
//                 .FirstOrDefaultAsync(s => s.SchoolId == schoolId);

//             // EDIT 3: Check for NULL first, THEN filter
//             if (school == null) return NotFound();

//             // Filter the list in memory so the user only sees Approved items
//             school.Needs = school.Needs.Where(n => n.IsApproved == true).ToList();
            
//             return Ok(school);
//         }
//     }
// }