using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;
using SchoolDonation.API.Models;
using SchoolDonation.API.Services;

namespace SchoolDonation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsedItemsController : ControllerBase
    {
        private readonly SchoolDonationContext _context;
        private readonly FileService _fileService;

        public UsedItemsController(SchoolDonationContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

		// 1. POST: api/UsedItems/Donate (Donor Only)
		[HttpPost("Donate")]
		[Authorize(Roles = "Donor")]
		public async Task<IActionResult> DonateItemJson(
	 [FromBody] CreateUsedItemJsonDto request)
		{
			if (string.IsNullOrWhiteSpace(request.Name) ||
				string.IsNullOrWhiteSpace(request.Description))
			{
				return BadRequest("Name and Description are required");
			}

			int donorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var item = new UsedItem
			{
				DonorId = donorId,
				Name = request.Name,
				Description = request.Description,
				ImageUrl = null,
				DonationDate = DateTime.Now,
				Status = "Available"
			};

			_context.UsedItems.Add(item);
			await _context.SaveChangesAsync();

			return Ok(new { Message = "Item added successfully!" });
		}


		// 2. GET: api/UsedItems/Available (Public/Manager)
		[HttpGet("Available")]
        public async Task<ActionResult<List<UsedItemCardDto>>> GetAvailableItems()
        {
            var items = await _context.UsedItems
                .Include(i => i.Donor)
                .Where(i => i.Status == "Available")
                .OrderByDescending(i => i.DonationDate) // <--- Fixed line
                .Select(i => new UsedItemCardDto
                {
                    ItemId = i.ItemId,
                    DonorName = i.Donor.FirstName + " " + i.Donor.LastName.Substring(0, 1) + ".",
                    DonorPhone = i.Donor.PhoneNumber,
					Name = i.Name,
                    Description = i.Description,
                    // Handle null images safely
                    ImageUrl = i.ImageUrl ?? "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?q=80&w=800",
                    // Handle null dates safely
                    Date = i.DonationDate ?? DateTime.Now,
                    Status = i.Status
                })
                .ToListAsync();

            return Ok(items);
        }
        // 3. POST: api/UsedItems/Claim/5 (Manager Only)
        [HttpPost("Claim/{itemId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ClaimItem(int itemId)
        {
            // Get Manager ID
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Find Manager's School
            var school = await _context.Schools.FirstOrDefaultAsync(s => s.ManagerId == userId);
            if (school == null) return BadRequest("You don't have a school profile.");

            // Find Item
            var item = await _context.UsedItems.FindAsync(itemId);
            if (item == null) return NotFound();
            if (item.Status != "Available") return BadRequest("Item is already taken.");

            // Assign Item
            item.SchoolId = school.SchoolId;
            item.Status = "Reserved"; // Reserved until delivery is confirmed

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"You have successfully reserved '{item.Name}' for {school.Name}." });
        }
    }
}