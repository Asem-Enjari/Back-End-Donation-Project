namespace SchoolDonation.API.DTOs
{
	// Donor uses this to upload an item
	public class CreateUsedItemJsonDto
	{
		public string? Name { get; set; }
		public string? Description { get; set; }
	}


	// Manager/Public sees this in the list
	public class UsedItemCardDto
    {
        public int ItemId { get; set; }
        public string DonorName { get; set; }
        public string Name { get; set; }
        public string DonorPhone { get; set; }
		public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } // "Available", "Reserved", "Taken"
    }
}