namespace SchoolDonation.API.DTOs
{
    public class CreateNeedDto
    {
		public string Title { get; set; } = null!;
		public string? Description { get; set; }
		public decimal Cost { get; set; }
	}
}
