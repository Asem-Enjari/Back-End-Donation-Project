namespace SchoolDonation.API.DTOs
{
	public class PaymentDto
	{
		public string SchoolName { get; set; } = null!;

		public string NeedTitle { get; set; } = null!;

		public decimal Amount { get; set; }

		public string? TransactionReference { get; set; }

		public string DonorEmail { get; set; }
	}
}
