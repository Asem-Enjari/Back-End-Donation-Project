using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly SchoolDonationContext _context;

        public MapController(SchoolDonationContext context)
        {
            _context = context;
        }

        // GET: api/Map/Pins
        [HttpGet("Pins")]
        public async Task<ActionResult<List<MapPinDto>>> GetMapPins()
        {
            // We join Schools with SchoolLocations
            // We verify that Location is not null (some schools might not have coordinates yet)
            var pins = await _context.Schools
                .Include(s => s.SchoolLocation)
                .Where(s => s.SchoolLocation != null) 
                .Select(s => new MapPinDto
                {
                    SchoolId = s.SchoolId,
                    Name = s.Name,
                    Status = s.Status ?? "Unknown",
                    
                    Lat = (double)s.SchoolLocation!.Latitude,
                    Lng = (double)s.SchoolLocation.Longitude
                })
                .ToListAsync();

            return Ok(pins);
        }
    }
}