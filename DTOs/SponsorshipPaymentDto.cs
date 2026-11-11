namespace SchoolDonation.API.DTOs
{
	public class SponsorshipPaymentDto
	{
		public string SchoolName { get; set; } = null!;
		public string ItemType { get; set; } = null!;
		public decimal Amount { get; set; }
		public string? TransactionReference { get; set; }
	}
}
