using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Data;
using SchoolDonation.API.DTOs;
using SchoolDonation.API.Models;
using System.Security.Claims;

namespace SchoolDonation.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class PaymentsController : ControllerBase
	{
		private readonly SchoolDonationContext _context;

		public PaymentsController(SchoolDonationContext context)
		{
			_context = context;
		}

		// GET: api/Payments
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
		{
			return await _context.Payments
				.Include(p => p.Need)
				.Include(p => p.School)
				.ToListAsync();
		}


		[HttpPost("sponsorship")]
		[Authorize(Roles = "Donor")]
		public async Task<IActionResult> PayForSponsorship([FromBody] SponsorshipPaymentDto dto)
		{
			int donorId = int.Parse(
				User.FindFirst(ClaimTypes.NameIdentifier)!.Value
			);

			// 1️⃣ Get School
			var school = await _context.Schools
				.FirstOrDefaultAsync(s => s.Name == dto.SchoolName);

			if (school == null)
				return NotFound("School not found.");

			// 2️⃣ Get Pending Sponsorship
			var sponsorship = await _context.StudentSponsorships
				.FirstOrDefaultAsync(s =>
					s.SchoolId == school.SchoolId &&
					s.ItemType == dto.ItemType &&
					s.Status == "Pending"
				);

			if (sponsorship == null)
				return NotFound("No pending sponsorship found for this school and item type.");

			// 3️⃣ Update Sponsorship
			sponsorship.Status = "Completed";
			sponsorship.IsApproved = true;
			sponsorship.DonorId = donorId;
			sponsorship.SponsorshipDate = DateTime.Now;

			// 4️⃣ Create Payment
			//var payment = new Payment
			//{
			//	SchoolId = school.SchoolId,
			//	NeedId = 0, // Sponsorship doesn't relate to Need
			//	Amount = dto.Amount,
			//	PaymentDate = DateTime.Now,
			//	TransactionReference = dto.TransactionReference
			//};

			//_context.Payments.Add(payment);

			await _context.SaveChangesAsync();

			return Ok(new
			{
				Message = "Sponsorship payment completed successfully.",
				SponsorshipId = sponsorship.SponsorId,
				School = school.Name,
				ItemType = sponsorship.ItemType,
				Amount = dto.Amount,
				Status = sponsorship.Status
			});
		}


		// POST: api/Payments
		[HttpPost]
		public async Task<IActionResult> AddPayment([FromBody] PaymentDto dto)
		{
			var need = await _context.Needs
				.Include(n => n.School)
				.FirstOrDefaultAsync(n =>
					n.Title == dto.NeedTitle &&
					n.School.Name == dto.SchoolName
				);

			if (need == null)
				return NotFound($"Need '{dto.NeedTitle}' in School '{dto.SchoolName}' not found.");

			var payment = new Payment
			{
				NeedId = need.NeedId,
				SchoolId = need.School.SchoolId,
				Amount = dto.Amount,
				PaymentDate = DateTime.Now,
				TransactionReference = dto.TransactionReference
			};
			_context.Payments.Add(payment);

			var donor = _context.Users.FirstOrDefault(u => u.Email == dto.DonorEmail);

			var donation = new Donation
			{
				NeedId = need.NeedId,
				Amount = dto.Amount,
				DonationDate = DateTime.Now,
				TransactionReference = dto.TransactionReference,
				DonorId = donor.UserId
			};
			_context.Donations.Add(donation);

			need.CollectedAmount ??= 0;
			need.CollectedAmount += dto.Amount;

			need.Status = (need.CollectedAmount >= need.Cost) ? "Completed" : "Active";

			await _context.SaveChangesAsync();

			return Ok(new
			{
				paymentId = payment.PaymentId,
				paymentDate = payment.PaymentDate,
				amount = payment.Amount,
				schoolName = need.School.Name,
				needTitle = need.Title,
				collectedAmount = need.CollectedAmount,
				needStatus = need.Status
			});
		}
	}
}
